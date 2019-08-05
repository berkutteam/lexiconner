﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using Mongo2Go;
using MongoDB.Driver;
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Domain.Enums;
using Lexiconner.Api.IntegrationTests.Auth;
using Lexiconner.Application.ApiClients;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Lexiconner.Api.IntegrationTests.Utils
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IDisposable where TStartup : class
    {
        private static readonly string _namespace = typeof(CustomWebApplicationFactory<TStartup>).Namespace;
        private static readonly string _appName = _namespace;

        public CustomWebApplicationFactory() : base()
        {
            // Do "global" initialization here; Only called once (for every class under test).
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            // Rewrite env variables for sure
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            Environment.SetEnvironmentVariable("Environment", "Testing");

            Log.Information("Configuring web host ({ApplicationContext})...", _appName);
            var builder = WebHost.CreateDefaultBuilder()
                .UseEnvironment("Testing")
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<TStartup>()
                .UseSerilog();

            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //// When using Autofac and returning its service provider in ConfigureServices
            //// using below methods can cause an exception. Be careful.

            //// !!! Called before the Startup's service configuration and get overwritten by it
            //builder.ConfigureServices(services =>
            //{
            //    Log.Information("ConfigureServices in {ApplicationContext}", _appName);
            //});

            // !!! Called after the Startup's service configuration and override it
            builder.ConfigureTestServices(services =>
            {
                Log.Information("ConfigureTestServices in {ApplicationContext}", _appName);

                var serviceProvider = services.BuildServiceProvider();
                ApplicationSettings config = serviceProvider.GetService<IOptions<ApplicationSettings>>().Value;
                IHostingEnvironment hostingEnvironment = serviceProvider.GetService<IHostingEnvironment>();

                //// use MongoDb as repository

                services.AddSingleton<MongoDbRunner>(sp => {
                    return MongoDbRunner.Start();
                });
                /*
                * Typically you only create one MongoClient instance for a given cluster and use it across your application. 
                * Creating multiple MongoClients will, however, still share the same pool of connections if and only if the connection strings are identical.
                */
                services.AddTransient<MongoClient>(sp => {
                    var mongoDbRunner = sp.GetService<MongoDbRunner>();
                    return new MongoClient(mongoDbRunner.ConnectionString);
                });

                // main repository
                services.AddTransient<IMongoRepository>(sp =>
                {
                    var mongoClient = sp.GetService<MongoClient>();
                    ILogger<IMongoRepository> logger = Mock.Of<ILogger<IMongoRepository>>();
                    return new MongoRepository(
                        mongoClient,
                        config.MongoDb.Database,
                        ApplicationDb.Main
                    );
                });
                services.AddTransient<IIdentityRepository, IdentityRepository>(sp =>
                {
                    var mongoClient = sp.GetService<MongoClient>();
                    ILogger<IMongoRepository> logger = Mock.Of<ILogger<IMongoRepository>>();
                    return new IdentityRepository(
                        mongoClient, 
                        config.MongoDb.DatabaseIdentity, 
                        ApplicationDb.Identity
                    );
                });

                // abstracted repository
                // do not register, because it will rewrite CosmosDb implementation above
                //services.AddTransient<IDataRepository>(sp =>
                //{
                //    return sp.GetService<IMongoDbDataRepository>();
                //});

                services.AddTransient<IGoogleTranslateApiClient, GoogleTranslateApiClientMock>(sp => {
                    return new GoogleTranslateApiClientMock();
                });
                services.AddTransient<IContextualWebSearchApiClient, ContextualWebSearchApiClientMock>(sp =>
                {
                    return new ContextualWebSearchApiClientMock();
                });

                // use mocked auth
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = TestAuthenticationDefaults.AuthenticationScheme;
                }).AddTestAuthentication(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = args =>
                        {
                            return Task.FromResult(0);
                        },
                    };
                });

                // use AddSingleton to acess the same object later usign DI
                // and be able to use Moq to check number of calls, for example
            });
        }

        public new void Dispose()
        {
            // Do "global" teardown here; Only called once (for every class under test).
            var dataUtil = new DataUtil<TStartup>(this);
            dataUtil.CleanUpAsync().GetAwaiter().GetResult();

            var mongoDbRunner = this.Server.Host.Services.GetService<MongoDbRunner>();
            mongoDbRunner.Dispose();

            // call base dispose as we rewritting Dispose
            base.Dispose();
        }
    }
}
