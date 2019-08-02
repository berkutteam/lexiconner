using Lexiconner.Application.ApiClients;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Domain.Enums;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.Persistence.Cache;
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Seed.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Lexiconner.Seed
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Data seed application: Started");

            bool replaceDatabase = false;

            Stopwatch watch = new Stopwatch();
            watch.Restart();

            MainAsync(replaceDatabase)
                .GetAwaiter()
                .GetResult();

            watch.Stop();


            Console.WriteLine($"Data seed application: Completed {watch.Elapsed}");
        }

        private static async Task MainAsync(bool replaceDatabase)
        {
            Console.WriteLine(HostingEnvironmentHelper.Environment);
            var configuration = BuildConfiguration();
            var serviceProvider = RegisterServices(configuration);
            var logger = serviceProvider.GetService<ILogger<Program>>();

            // Log parameters
            logger.LogInformation("\n");
            logger.LogInformation("Parameters:");
            logger.LogInformation("Environment: {Environment}", HostingEnvironmentHelper.Environment);
            logger.LogInformation("\n");

            var seedService = serviceProvider.GetService<ISeedService>();
            var mongoRepository = serviceProvider.GetService<IMongoRepository>();

            if (replaceDatabase)
            {
                await seedService.RemoveDatabaseAsync();
            }

            // configure collections (set indexes, ...)
            logger.LogInformation("Configure collections (set indexes, ...)");
            await mongoRepository.InitializeCollectionAsync<GoogleTranslateDataCacheEntity>();
            await mongoRepository.InitializeCollectionAsync<GoogleTranslateDetectLangugaeDataCacheEntity>();
            await mongoRepository.InitializeCollectionAsync<ContextualWebSearchImageSearchDataCacheEntity>();

            // seed
            await seedService.SeedAsync();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            // load env variables from .env file
            string envFilePath = Path.Combine(Directory.GetCurrentDirectory(), $".env__{HostingEnvironmentHelper.Environment}");
            if (File.Exists(envFilePath))
            {
                DotNetEnv.Env.Load(envFilePath);
            }

            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"appsettings.{HostingEnvironmentHelper.Environment}.json", optional: true, reloadOnChange: true);
            builder.AddEnvironmentVariables();

            return builder.Build();
        }

        private static IServiceProvider RegisterServices(IConfigurationRoot configuration)
        {
            var config = configuration.Get<ApplicationSettings>();
            var services = new ServiceCollection();

            services.AddOptions();
            services.Configure<ApplicationSettings>(configuration);

            // Override the current ILogger implementation to use Serilog
            services.AddLogging(configure => configure.AddSerilog());

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

            services.AddTransient<IDataCache, DataCacheDataRepository>();
            services.AddTransient<IImageService, ImageService>();

            services.AddTransient<IWordTxtImporter, WordTxtImporter>(xp => {
                return new WordTxtImporter(config.Import);
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

            services.AddTransient<IIdentityServerConfig, IdentityServerConfig>();

            if (HostingEnvironmentHelper.IsDevelopmentLocalhost())
            {
                services.AddTransient<ISeedService, SeedServiceDevelopmentLocalhost>();
            }
            else if (HostingEnvironmentHelper.IsDevelopmentHeroku())
            {
                services.AddTransient<ISeedService, SeedServiceDevelopmentHeroku>();
            }
            else if (HostingEnvironmentHelper.IsProductionHeroku())
            {
                services.AddTransient<ISeedService, SeedServiceProductionHeroku>();
            }

            Log.Logger = GetSerilogLogger();

            return services.BuildServiceProvider();
        }

        private static Serilog.ILogger GetSerilogLogger()
        {
            LoggerConfiguration logger = new LoggerConfiguration()
               .WriteTo.Console();

            return logger.CreateLogger();
        }
    }
}
