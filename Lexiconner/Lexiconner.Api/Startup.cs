using Autofac;
using AutoMapper;
using FluentValidation.AspNetCore;
using Lexiconner.Api.Attributes;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Scrapers;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.Mapping;
using Lexiconner.Application.Middlewares;
using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence;
using Lexiconner.Persistence.Cache;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TMDbLib.Client;

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
                    sp.GetService<ILogger<IGoogleTranslateApiClient>>(),
                     sp.GetService<IHttpClientFactory>()
                );
            });
            services.AddTransient<IContextualWebSearchApiClient, ContextualWebSearchApiClient>(sp =>
            {
                return new ContextualWebSearchApiClient(
                    config.RapidApi,
                    sp.GetService<ILogger<IContextualWebSearchApiClient>>(),
                     sp.GetService<IHttpClientFactory>()
                );
            });
            services.AddTransient<ITwinwordWordDictionaryApiClient, TwinwordWordDictionaryApiClient>(sp =>
            {
                return new TwinwordWordDictionaryApiClient(
                    config.RapidApi.TwinwordWordDictionary,
                    sp.GetService<ILogger<ITwinwordWordDictionaryApiClient>>(),
                     sp.GetService<IHttpClientFactory>()
                );
            });
            services.AddSingleton<TMDbClient>(sp => 
            {
                return new TMDbClient(config.TheMovieDatabase.ApiKeyV3Auth);    
            });
            services.AddTransient<IReversoContextScraper, ReversoContextScraper>();
            services.AddTransient<IOxfordLearnersDictionariesScrapper, OxfordLearnersDictionariesScrapper>();

            services.AddTransient<IDataCache, DataCacheDataRepository>(sp => {
                var logger = sp.GetService<ILogger<IDataCache>>();
                ISharedCacheDataRepository dataRepository = sp.GetService<ISharedCacheDataRepository>();
                return new DataCacheDataRepository(logger, dataRepository);
            });

            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IWordsService, WordsService>();
            services.AddTransient<IWordSetsService, WordSetsService>();
            services.AddTransient<IWordTrainingsService, WordTrainingsService>();
            services.AddTransient<ICustomCollectionsService, CustomCollectionsService>();
            services.AddTransient<IFilmsService, FilmsService>();
            services.AddTransient<IUsersService, UsersService>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton<IMapper>(mapper);

            services.AddAuthorization();
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = config.JwtBearerAuth.Authority;
                    options.RequireHttpsMetadata = false;
                    options.Audience = config.JwtBearerAuth.Audience;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // from IdentityServer v4 don't validate Audience, but validae scopes
                        ValidateAudience = false
                    };

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

            // validate api scope claim is present (starting from IdentityServer v4)
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DefaultWebApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", config.JwtBearerAuth.WebApiScope);
                });
            });

            services.AddSwaggerGen(options =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                //Here we set the response schema for DateTime and DateTimeOffset
                //We are using ISO 8601 format for DateTime strings in response.
                options.MapType<DateTime>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date-time",
                    Description =
                        @"Date-time string in <a href=""https://en.wikipedia.org/wiki/ISO_8601#UTC\"">ISO 8601 format</a>."
                });
                options.MapType<DateTimeOffset>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date-time-offset",
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            //var ssss = app.ApplicationServices.GetService<IGoogleTranslateApiClient>();
            //ssss.Translate("", "", "").GetAwaiter().GetResult();
            //var scraper = app.ApplicationServices.GetService<IReversoContextScraper>();
            //scraper.GetWordTranslationAsync("cat", "en", "ru").Wait();

            // Request/Response logging middleware
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

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
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization("DefaultWebApiScope");
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
            MongoDbEntityMapper.ConfigureMapping();

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
