// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Lexiconner.IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Lexiconner.IdentityServer4.Extensions;
using Lexiconner.IdentityServer4.Services;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Lexiconner.Domain.Entitites;
using MongoDB.Driver;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Lexiconner.Application.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Extensions;
using Microsoft.IdentityModel.Logging;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Application.Helpers;
using System.Reflection;
using System.IO;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using System.Collections.Generic;
using Serilog;
using Autofac;
using Lexiconner.Persistence;
using Lexiconner.Application.Middlewares;
using IdentityServer4;

namespace Lexiconner.IdentityServer4
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.Get<ApplicationSettings>();

            services.AddOptions();
            services.AddHttpClient();
            services.Configure<ApplicationSettings>(Configuration);

            ConfigureLogger(services);
            ConfigureMongoDb(services);

            //services.AddIdentity<ApplicationUserEntity, ApplicationRoleEntity>(options =>
            //{
            //    if (Environment.IsDevelopmentAny())
            //    {
            //        options.Password.RequireDigit = false;
            //        options.Password.RequireUppercase = false;
            //        options.Password.RequireNonAlphanumeric = false;
            //        options.Password.RequiredLength = 4;
            //    }
            //})
            //.AddDefaultTokenProviders();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            // configure identity server with MONGO Repository for stores, keys, clients, scopes & Asp .Net Identity
            var identityServerBuilder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });

            // Fix IdentityServe4 and new SameSite Cookie policy
            services.ConfigureNonBreakingSameSiteCookies();

            identityServerBuilder.AddSigningCredentialCustom(Environment, config);
            identityServerBuilder.AddConfig()
            //.CheckMongoDataRepository()
            .AddMongoDbForAspIdentity<ApplicationUserEntity, ApplicationRoleEntity>(config)
            .AddClients()
            .AddIdentityApiResources()
            .AddPersistedGrants()
            .AddAspNetIdentity<ApplicationUserEntity>()
            .AddProfileService<ProfileService>();

            services
                .AddAuthentication()
                .AddGoogle(options =>
                {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5004/signin-google
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = config.GoogleAuthentication.ClientId;
                    options.ClientSecret = config.GoogleAuthentication.ClientSecret;
                });

            if (Environment.IsDevelopmentAny())
            {
                IdentityModelEventSource.ShowPII = true; // show detail of error and see the problem
            }

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

            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ApiExceptionFilterAttribute>();
            })
            .AddNewtonsoftJson(options =>
            {
                // SerializationConfig.GetDefaultJsonSerializerSettings(options.SerializerSettings);
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

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            // Request/Response logging middleware
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            // fix for new Cookie policy https://web.dev/samesite-cookies-explained/
            // IdentityServer can't login without this setting
            // https://stackoverflow.com/questions/60757016/identity-server-4-post-login-redirect-not-working-in-chrome-only
            //app.UseCookiePolicy(new CookiePolicyOptions
            //{
            //    MinimumSameSitePolicy = SameSiteMode.Lax
            //});
            app.UseCookiePolicy();

            if (Environment.IsDevelopmentAny())
            {
                app.UseDeveloperExceptionPage();
            }

            if (Environment.IsDevelopmentHeroku() || Environment.IsProductionHeroku())
            {
                // resolve http instead https issue in '/.well-known/openid-configuration'
                // maybe heroku uses some proxy and app gets http requests instead of https
                var forwardOptions = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
                    // RequireHeaderSymmetry seems to be false default in 2.1
                    RequireHeaderSymmetry = false
                };

                // Clear the forward headers networks so any ip can forward headers
                // Should ONLY do this in dev/testing
                forwardOptions.KnownNetworks.Clear();
                forwardOptions.KnownProxies.Clear();

                // For security you should limit the networks that can forward headers
                // forwardOptions.KnownNetworks.Add(new IPNetwork());

                app.UseForwardedHeaders(forwardOptions);


                ////// approach with custom middleware (try if above solution with UseForwardedHeaders doesn't work
                //const string XForwardedPathBase = "X-Forwarded-PathBase";
                //const string XForwardedProto = "X-Forwarded-Proto";
                //app.Use((context, next) => {
                //    if (context.Request.Headers.TryGetValue(XForwardedPathBase, out StringValues pathBase))
                //    {
                //        context.Request.PathBase = new PathString(pathBase);

                //    }

                //    // if this is commented out, identity server urls are http://
                //    if (context.Request.Headers.TryGetValue(XForwardedProto, out StringValues proto))
                //    {
                //        context.Request.Protocol = proto;
                //    }

                //    // this was not needed, problem was above
                //    //string origin = context.Request.Scheme + "://" + context.Request.Host.Value;
                //    //context.SetIdentityServerOrigin(origin);

                //    return next();
                //});
                //////

                app.UseHsts();
                if (!HostingEnvironmentHelper.IsDevelopmentLocalhost() && !HostingEnvironmentHelper.IsTestingAny())
                {
                    app.UseHttpsRedirection();
                }
            }

            if (Environment.IsProductionAny())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCors("default");

            // UseRouting must go before any authorization. Otherwise authorization won't work properly.
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // UseIdentityServer includes a call to UseAuthentication, so it’s not necessary to have both.
            app.UseIdentityServer();

            // Configure Google Auth
            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AuthenticationScheme = "Google",
            //    DisplayName = "Google",
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

            //    ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com",
            //    ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo",
            //    Scope = { "openid", "profile", "email" }
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}"
               );
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
            //    return new MongoDataRepository(mongoClient, config.MongoDb.Database, ApplicationDb.Identity);
            //});

            // abstracted repository
            services.AddTransient<IDataRepository, MongoDataRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new MongoDataRepository(mongoClient, config.MongoDb.Database, ApplicationDb.Identity);
            });
        }
    }
}