using Microsoft.AspNetCore;
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
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Domain.Enums;
using Lexiconner.Api.IntegrationTests.Auth;
using Lexiconner.Application.ApiClients;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence;
using Lexiconner.Application.Helpers;
using Microsoft.Extensions.Hosting;

namespace Lexiconner.Api.IntegrationTests.Utils
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IDisposable where TStartup : class
    {
        private static readonly string _namespace = typeof(CustomWebApplicationFactory<TStartup>).Namespace;
        private static readonly string _appName = _namespace;

        public CustomWebApplicationFactory() : base()
        {
            // Do "global" initialization here; Only called once (for every class under test).

            // NB: The WebApplicationFactory description says that it searches for TEntryPoint (i.e. TStartup),
            // which in our case can be the original Startup, from the source project that is tested, but with some rewrites.
            // This means it pick ups CreateWebHostBuilder method which is in Program.cs, so all the code in Program.CreateWebHostBuilder is called
            // when running tests but Program.Main isn't called. 
            // Keep this in mind.

            // Rewrite env variables for sure
            if (
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) ||
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Environment")) ||
                !HostingEnvironmentHelper.IsTestingAny()
            )
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", HostingEnvironmentHelper.HostingEnvironmentDefaults.TestingLocalhost);
                Environment.SetEnvironmentVariable("Environment", HostingEnvironmentHelper.HostingEnvironmentDefaults.TestingLocalhost);
            }
            if (!HostingEnvironmentHelper.IsTestingAny())
            {
                throw new Exception("Environment must set to any testing env.");
            }
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            Log.Information("Creating host ({ApplicationContext})...", _appName);
            return base.CreateHostBuilder();
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            // Not executed for .NET Core 3.0+
            // The reason for this is that WebApplicationFactory supports both the legacy WebHost and the generic Host.
            // If the app you're testing uses a WebHostBuilder in Program.cs, then the factory calls CreateWebHostBuilder() and runs the overridden method. 
            // However if the app you're testing uses the generic HostBuilder, then the factory calls a different method, CreateHostBuilder().

            Log.Information("Creating web host ({ApplicationContext})...", _appName);
            return base.CreateWebHostBuilder();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Executed after Host created

            // When using Autofac and returning its service provider in ConfigureServices
            // using below methods can cause an exception. Be careful.

            // Executed before the Startup.ConfigureServices and is overwritten by it
            // From ASP.NET Core 3.0 executed after Startup.ConfigureServices code is executed.
            builder.ConfigureServices(services =>
            {
                Log.Information("ConfigureServices in {ApplicationContext}", _appName);
            });

            // Executed after the Startup.ConfigureServices and is overwritten by it
            builder.ConfigureTestServices(services =>
            {
                Log.Information("ConfigureTestServices in {ApplicationContext}", _appName);

                var serviceProvider = services.BuildServiceProvider();
                ApplicationSettings config = serviceProvider.GetService<IOptions<ApplicationSettings>>().Value;

                ConfigureMongoDb(services);

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

        private void ConfigureMongoDb(IServiceCollection services)
        {
            MongoDbEntityMapper.ConfigureMapping();

            // add Mongo2Go
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
            // no need. Just cast IDataRepository to IMongoDataRepository if needed
            //services.AddTransient<IMongoDataRepository, MongoDataRepository>(sp =>
            //{
            //    var mongoClient = sp.GetService<MongoClient>();
            //    ApplicationSettings config = sp.GetService<IOptions<ApplicationSettings>>().Value;
            //    ILogger<IMongoDataRepository> logger = Mock.Of<ILogger<IMongoDataRepository>>();
            //    return new MongoDataRepository(
            //        mongoClient,
            //        config.MongoDb.Database,
            //        ApplicationDb.Main
            //    );
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
