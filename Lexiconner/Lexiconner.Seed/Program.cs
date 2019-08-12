using Lexiconner.Application.ApiClients;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Domain.Enums;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.Persistence.Cache;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Seed.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            var mongoDataRepository = serviceProvider.GetService<IMongoDataRepository>();

            if (replaceDatabase)
            {
                await seedService.RemoveDatabaseAsync();
            }

            // configure collections (set indexes, ...)
            logger.LogInformation("Configure collections (set indexes, ...)");
            await mongoDataRepository.InitializeCollectionAsync<GoogleTranslateDataCacheEntity>();
            await mongoDataRepository.InitializeCollectionAsync<GoogleTranslateDetectLangugaeDataCacheEntity>();
            await mongoDataRepository.InitializeCollectionAsync<ContextualWebSearchImageSearchDataCacheEntity>();

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

            ConfigureMongoDb(services);
           
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

        private static void ConfigureMongoDb(IServiceCollection services)
        {
            /*
            * Typically you only create one MongoClient instance for a given cluster and use it across your application. 
            * Creating multiple MongoClients will, however, still share the same pool of connections if and only if the connection strings are identical.
           */
            services.AddTransient<MongoClient>(sp => {
                ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new MongoClient(config.MongoDb.ConnectionString);
            });

            // main repository
            services.AddTransient<IMongoDataRepository, MongoDataRepository>(sp =>
            {
                var mongoClient = sp.GetService<MongoClient>();
                ApplicationSettings config = sp.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new MongoDataRepository(mongoClient, config.MongoDb.Database, ApplicationDb.Main);
            });

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
    }
}
