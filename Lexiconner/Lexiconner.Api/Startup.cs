using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.Base;
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

namespace Lexiconner.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.Get<ApplicationSettings>();

            services.AddOptions();
            services.AddHttpClient();
            services.Configure<ApplicationSettings>(Configuration);

            /*
             * Typically you only create one MongoClient instance for a given cluster and use it across your application. 
             * Creating multiple MongoClients will, however, still share the same pool of connections if and only if the connection strings are identical.
            */
            services.AddTransient<MongoClient>(serviceProvider => {
                return new MongoClient(config.MongoDb.ConnectionString);
            });

            services.AddTransient<IMongoRepository, MongoRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                return new MongoRepository(mongoClient, config.MongoDb.Database, ApplicationDb.Main);
            });
            services.AddTransient<IIdentityRepository, IdentityRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                return new IdentityRepository(mongoClient, config.MongoDb.DatabaseIdentity, ApplicationDb.Identity);
            });
            services.AddTransient<IGoogleTranslateApiClient, GoogleTranslateApiClient>(sp => {
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

            services.AddTransient<IDataCache, DataCacheDataRepository>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IStudyItemsService, StudyItemsService>();

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
                    options.RequireHttpsMetadata = true;
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

            services.AddSwaggerGen();

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

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            //var ssss = app.ApplicationServices.GetService<IGoogleTranslateApiClient>();
            //ssss.Translate("", "", "").GetAwaiter().GetResult();

            if (env.IsDevelopmentAny())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(
               options =>
               {
                   foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                   {
                       options.SwaggerEndpoint(
                           $"/swagger/{description.GroupName}/swagger.json",
                           description.GroupName.ToUpperInvariant());
                   }
               });
        }
    }
}
