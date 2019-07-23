// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Lexiconner.IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Lexiconner.IdentityServer4.Extensions;
using Lexiconner.IdentityServer4.Services;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Lexiconner.Domain.Entitites;
using MongoDB.Driver;
using Lexiconner.Persistence.Repositories.Base;
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

namespace Lexiconner.IdentityServer4
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.Get<ApplicationSettings>();

            services.AddOptions();
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
                return new MongoRepository(mongoClient, config.MongoDb.Database);
            });

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
            identityServerBuilder.AddSigningCredentialCustom(Environment, config);
            identityServerBuilder.AddConfig()
            .AddMongoRepository()
            .AddMongoDbForAspIdentity<ApplicationUserEntity, ApplicationRoleEntity>(config)
            .AddClients()
            .AddIdentityApiResources()
            .AddPersistedGrants()
            .AddAspNetIdentity<ApplicationUserEntity>();
            //.AddTestUsers(Config.GetUsers())
            //.AddProfileService<ProfileService>();

            services.AddAuthentication();
            //.AddGoogle(options =>
            //{
            //    // register your IdentityServer with Google at https://console.developers.google.com
            //    // enable the Google+ API
            //    // set the redirect URI to http://localhost:5000/signin-google
            //    options.ClientId = "copy client ID from Google here";
            //    options.ClientSecret = "copy client secret from Google here";
            //});

            if (Environment.IsDevelopmentAny())
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

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            if (Environment.IsDevelopmentAny())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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
                app.UseHttpsRedirection();
            }

            if (Environment.IsProductionAny())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCors("default");

            // UseIdentityServer includes a call to UseAuthentication, so it’s not necessary to have both.
            app.UseIdentityServer();
            app.UseMongoDbForIdentityServer();

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

            app.UseMvcWithDefaultRoute();

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