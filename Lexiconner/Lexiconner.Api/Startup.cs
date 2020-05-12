using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.ApiClients;
using System.Net.Http;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Cache;
using Lexiconner.Application.Services;
using Lexiconner.Api.Services;
using Autofac;
using Lexiconner.Application.Helpers;
using FluentValidation.AspNetCore;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Api.Attributes;
using Lexiconner.Domain.Config;
using System;
using System.IO;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Lexiconner.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.Get<ApplicationSettings>();

            services.AddOptions();
            services.AddHttpClient();
            services.Configure<ApplicationSettings>(Configuration);

            ConfigureLogger(services);
            ConfigureMongoDb(services);

            services.AddTransient<IGoogleTranslateApiClient, GoogleTranslateApiClient>(sp =>
            {
                return new GoogleTranslateApiClient(
                    config.Google.ProjectId,
                    config.Google.WebApiServiceAccount,
                    sp.GetService<ILogger<IGoogleTranslateApiClient>>()
                );
            });
            services.AddTransient<IContextualWebSearchApiClient, ContextualWebSearchApiClient>(sp =>
            {
                return new ContextualWebSearchApiClient(
                    config.RapidApi,
                    sp.GetService<ILogger<IContextualWebSearchApiClient>>()
                );
            });

            services.AddTransient<IDataCache, DataCacheDataRepository>(sp => {
                var logger = sp.GetService<ILogger<IDataCache>>();
                ISharedCacheDataRepository dataRepository = sp.GetService<ISharedCacheDataRepository>();
                return new DataCacheDataRepository(logger, dataRepository);
            });
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IStudyItemsService, StudyItemsService>();
            services.AddTransient<ICustomCollectionsService, CustomCollectionsService>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            // setup Basic Http Auth
            //services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
            //    .AddBasicAuthentication(options =>
            //    {
            //        options.Realm = "My Api";
            //        options.Events = new BasicAuthenticationEvents()
            //        {
            //            OnValidatePrincipal = (context) =>
            //            {
            //                if ((context.UserName.ToLower() == config.BasicAuth.Username) && (context.Password == config.BasicAuth.Password))
            //                {
            //                    List<Claim> claims = new List<Claim>
            //                      {
            //                        new Claim(ClaimTypes.Name,
            //                                  context.UserName,
            //                                  context.Options.ClaimsIssuer)
            //                      };

            //                    var principal = new ClaimsPrincipal(new ClaimsIdentity(
            //                        claims,
            //                        BasicAuthenticationDefaults.AuthenticationScheme));
            //                    var ticket = new AuthenticationTicket(
            //                        principal,
            //                        new AuthenticationProperties(),
            //                        BasicAuthenticationDefaults.AuthenticationScheme
            //                    );

            //                    context.Principal = principal;

            //                    return Task.FromResult(AuthenticateResult.Success(ticket));
            //                }

            //                return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
            //            }
            //        };
            //    });

            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = config.JwtBearerAuth.Authority;
                    options.RequireHttpsMetadata = false;
                    options.Audience = config.JwtBearerAuth.Audience;
                    options.Events = new JwtBearerEvents
                    {
                        // https://stackoverflow.com/questions/48649717/addjwtbearer-onauthenticationfailed-return-custom-error
                        OnAuthenticationFailed = context =>
                        {
                            // by default handler automatically return a WWW-Authenticate response header containing an error code/description when a 401 response is returned
                            if (HostingEnvironment.IsDevelopmentAny())
                            {
                                // TODO: return here base model with message for client

                                // For debugging purposes only!
                                // If you change response body status will be 200 intead of 401
                                var text = $"AuthenticationFailed: {context.Exception.Message}";

                                //// resets default response and clears WWW-Authenticate header
                                //context.NoResult();
                                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                //context.Response.ContentType = "text/plain";
                                //context.Response.WriteAsync(text).GetAwaiter().GetResult();
                            }
                            return Task.CompletedTask;
                        },
                    };
                });

            if (HostingEnvironment.IsDevelopmentAny())
            {
                IdentityModelEventSource.ShowPII = true; // show detail of error and see the problem
            }

            services.AddSwaggerGen(options =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                //Here we set the response schema for DateTime
                //We are using ISO 8601 format for DateTime strings in response.
                options.MapType<DateTime>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date-time",
                    Description =
                        @"Date-time string in <a href=""https://en.wikipedia.org/wiki/ISO_8601#UTC\"">ISO 8601 format</a>."
                });

                options.EnableAnnotations();
            });

            services.AddApiVersioning(options =>
            {
                // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(
               options =>
               {
                   // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                   // note: the specified format code will format the version as "'v'major[.minor][-status]"
                   options.GroupNameFormat = "'v'VVV";

                   // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                   // can also be used to control the format of the API version in route templates
                   options.SubstituteApiVersionInUrl = true;
               });

            if (!HostingEnvironmentHelper.IsTestingAny())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("default", builder =>
                    {
                        builder
                            .WithOrigins(config.Cors.AllowedOrigins.ToArray())
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                });
            }

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            })
            .AddNewtonsoftJson(options =>
            {
                SerializationConfig.GetDefaultJsonSerializerSettings(options.SerializerSettings);
            })
            .AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssembly(typeof(Startup).Assembly); // register all validators in assembly
                options.RegisterValidatorsFromAssembly(typeof(Lexiconner.Domain.Anchor).Assembly); // register all validators in assembly
                options.RunDefaultMvcValidationAfterFluentValidationExecutes = true; // allow default validation to run
            });
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you. If you
        // need a reference to the container, you need to use the
        // "Without ConfigureContainer" mechanism (see docs).
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacDefaultModule());
        }

        // This method gets caled by the runtime. Use this method to configure the HTTP request pipeline.
        // This is called after ConfigureContainer.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            //var ssss = app.ApplicationServices.GetService<IGoogleTranslateApiClient>();
            //ssss.Translate("", "", "").GetAwaiter().GetResult();

            if (HostingEnvironmentHelper.IsDevelopmentAny())
            {
                // app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (!HostingEnvironmentHelper.IsDevelopmentLocalhost() && !HostingEnvironmentHelper.IsTestingAny())
            {
                app.UseHttpsRedirection();
            }

            if (!HostingEnvironmentHelper.IsTestingAny())
            {
                app.UseCors("default");
            }

            // UseRouting must go before any authorization. Otherwise authorization won't work properly.
            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "swagger/{documentName}/swagger.json";
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer
                        {
                            Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.PathBase}"
                        }
                    };
                });
            });
            app.UseSwaggerUI(
              options =>
              {
                  foreach (var description in provider.ApiVersionDescriptions)
                  {
                      options.SwaggerEndpoint(
                          $"/swagger/{description.GroupName}/swagger.json",
                          description.GroupName.ToUpperInvariant());
                  }
              });
        }

        private void ConfigureLogger(IServiceCollection services)
        {
            // Override the current ILogger implementation to use Serilog
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });
        }

        private void ConfigureMongoDb(IServiceCollection services)
        {
            /*
            * Typically you only create one MongoClient instance for a given cluster and use it across your application. 
            * Creating multiple MongoClients will, however, still share the same pool of connections if and only if the connection strings are identical.
           */
            services.AddTransient<MongoClient>(sp => {
                ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new MongoClient(config.MongoDb.ConnectionString);
            });

            // main repository
            // no need. Just cast IDataRepository to IMongoDataRepository if needed
            //services.AddTransient<IMongoDataRepository, MongoDataRepository>(sp =>
            //{
            //    var mongoClient = sp.GetService<MongoClient>();
            //    ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
            //    return new MongoDataRepository(mongoClient, config.MongoDb.Database, ApplicationDb.Main);
            //});

            // abstracted repository
            services.AddTransient<IDataRepository, MongoDataRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new MongoDataRepository(mongoClient, config.MongoDb.DatabaseMain, ApplicationDb.Main);
            });
            services.AddTransient<IIdentityDataRepository, IdentityDataRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new IdentityDataRepository(mongoClient, config.MongoDb.DatabaseIdentity, ApplicationDb.Identity);
            });
            services.AddTransient<ISharedCacheDataRepository, SharedCacheDataRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new SharedCacheDataRepository(mongoClient, config.MongoDb.DatabaseSharedCache, ApplicationDb.SharedCache);
            });
        }
    }
}
