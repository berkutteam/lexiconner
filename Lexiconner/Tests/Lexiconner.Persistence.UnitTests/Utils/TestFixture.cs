using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using Moq;
using NUlid;
using NUlid.Rng;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Repositories;

namespace Lexiconner.Persistence.UnitTests.Utils
{
    public class TestFixture : IDisposable
    {
        public ServiceProvider ServiceProvider = null;

        /// <summary>
        /// Used for proped cleanup
        /// </summary>
        public bool IsCosmosDbTestsRun { get; set; }

        public TestFixture() : base()
        {
            var services = ConfigureServices();
            ServiceProvider = services.BuildServiceProvider();

            // Do "global" initialization here; Only called once (for every class under test).
        }

        public ApplicationSettings GetConfig()
        {
            return ServiceProvider.GetService<IOptions<ApplicationSettings>>().Value;
        }

        private IServiceCollection ConfigureServices()
        {
            IConfiguration configuration = GetConfiguration();
            IServiceCollection services = new ServiceCollection();

            services.AddOptions();
            services.Configure<ApplicationSettings>(configuration);

            services.AddSingleton<ILogger<IMongoDataRepository>>(sp =>
            {
                return Mock.Of<ILogger<IMongoDataRepository>>();
            });
            services.AddTransient<DataUtil>();

            ConfigureMongoDb(services);

            return services;
        }

        private IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private void ConfigureMongoDb(IServiceCollection services)
        {
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
            //    ILogger<IMongoDataRepository> logger = Mock.Of<ILogger<IMongoDataRepository>>();
            //    ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
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
                return new MongoDataRepository(mongoClient, config.MongoDb.Database, ApplicationDb.Main);
            });
            services.AddTransient<IIdentityDataRepository, IdentityDataRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new IdentityDataRepository(mongoClient, config.MongoDb.DatabaseIdentity, ApplicationDb.Identity);
            });
        }

        public void Dispose()
        {
            // Do "global" teardown here; Only called once (for every class under test).
            var dataUtil = ServiceProvider.GetService<DataUtil>();

            dataUtil.CleanUpAsync().GetAwaiter().GetResult();
        }
    }
}
