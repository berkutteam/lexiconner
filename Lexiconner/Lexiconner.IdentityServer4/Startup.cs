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
using Lexiconner.IdentityServer4.Entities;
using Lexiconner.IdentityServer4.Services;

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

            //services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            //{
            //    if (Environment.IsDevelopment())
            //    {
            //        options.Password.RequireDigit = false;
            //        options.Password.RequireUppercase = false;
            //        options.Password.RequireNonAlphanumeric = false;
            //        options.Password.RequiredLength = 4;
            //    }
            //})
            //.AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            // configure identity server with MONGO Repository for stores, keys, clients, scopes & Asp .Net Identity
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddConfig()
            .AddMongoRepository()
            .AddMongoDbForAspIdentity<Lexiconner.IdentityServer4.Entities.ApplicationUser, Lexiconner.IdentityServer4.Entities.ApplicationRole>(config)
            .AddClients()
            .AddIdentityApiResources()
            .AddPersistedGrants()
            .AddAspNetIdentity<Lexiconner.IdentityServer4.Entities.ApplicationUser>();
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
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentityServer();
            app.UseMongoDbForIdentityServer();
            app.UseAuthentication(); // app.UseIdentity();

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
        }
    }
}