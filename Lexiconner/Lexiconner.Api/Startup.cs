using Lexiconner.Api.ImportAndExport;
using Lexiconner.Api.Seed;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Persistence.Repositories.Json;
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

            services.AddTransient<IWordTxtImporter, WordTxtImporter>();
            services.AddTransient<ISeeder, MongoDbSeeder>(); // replace with other if needed
            services.AddTransient<IStudyItemJsonRepository, StudyItemJsonRepository>(serviceProvider =>
            {
                // TODO use config
                return new StudyItemJsonRepository(Configuration.GetValue<string>("JsonStorePath"));
            });
            services.AddTransient<IMongoRepository, MongoRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                return new MongoRepository(mongoClient, config.MongoDb.Database);
            });
            services.AddTransient<IIdentityRepository, IdentityRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                return new IdentityRepository(mongoClient, config.MongoDb.DatabaseIdentity);
            });
            services.AddTransient<IGoogleTranslateApiClient, GoogleTranslateApiClient>(sp => {
                return new GoogleTranslateApiClient(
                    sp.GetRequiredService<IHostingEnvironment>(), 
                    config.Google.ProjectId,
                    config.Google.WebApiServiceAccount
                );
            });
            services.AddTransient<IContextualWebSearchApiClient, ContextualWebSearchApiClient>();

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
                        OnAuthenticationFailed = args =>
                        {
                            if (HostingEnvironment.IsDevelopmentAny())
                            {
                                return AuthenticationFailed(args);
                            }
                            return Task.FromResult(0);
                        },
                    };
                });

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
            var ssss = app.ApplicationServices.GetService<IGoogleTranslateApiClient>();
            ssss.Translate("", "", "").GetAwaiter().GetResult();

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

            // seed
            ISeeder seeder = app.ApplicationServices.GetRequiredService<ISeeder>();
            seeder.Seed().Wait();
        }

        private Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            var s = $"AuthenticationFailed: {arg.Exception.Message}";
            arg.Response.ContentLength = s.Length;
            arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            return Task.FromResult(0);
        }
    }
}
