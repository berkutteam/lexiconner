using Lexiconner.Application.ApiClients;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Domain.Enums;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.Persistence.Cache;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Seed.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbGenericRepository;
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
        // not changing unique app name.
        private static readonly string _appName = "FinancialPortfolio.Seed";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Data seed application: Started");

            bool replaceDatabase = false;

            Stopwatch watch = new Stopwatch();
            watch.Restart();

            await MainAsync(replaceDatabase);

            watch.Stop();


            Console.WriteLine($"Data seed application: Completed {watch.Elapsed}");
        }

        private static async Task MainAsync(bool replaceDatabase)
        {
            Console.WriteLine(HostingEnvironmentHelper.Environment);
            var configuration = BuildConfiguration();
            var config = configuration.Get<ApplicationSettings>();

            Log.Logger = GetSerilogLogger(configuration, config);

            var serviceProvider = RegisterServices(configuration);
            var logger = serviceProvider.GetService<ILogger<Program>>();

            // Log parameters
            logger.LogInformation("\n");
            logger.LogInformation("Parameters:");
            logger.LogInformation("Environment: {Environment}", HostingEnvironmentHelper.Environment);
            logger.LogInformation("\n");

            var seedService = serviceProvider.GetService<ISeedService>();
            var sharedCacheDataRepository = serviceProvider.GetService<ISharedCacheDataRepository>();
            var sharedCacheMongoDataRepository = sharedCacheDataRepository as IMongoDataRepository;

            if (replaceDatabase)
            {
                await seedService.RemoveDatabaseAsync();
            }

            // configure collections (set indexes, ...)
            logger.LogInformation("Configure collections (set indexes, ...)");
            await sharedCacheMongoDataRepository.InitializeCollectionAsync<GoogleTranslateDataCacheEntity>();
            await sharedCacheMongoDataRepository.InitializeCollectionAsync<GoogleTranslateDetectLangugaeDataCacheEntity>();
            await sharedCacheMongoDataRepository.InitializeCollectionAsync<ContextualWebSearchImageSearchDataCacheEntity>();

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
            services.Configure<Lexiconner.IdentityServer4.ApplicationSettings>(configuration); // map current config to identity  config

            // register directly to access using DI
            services.AddTransient<IConfigurationRoot>(sp => configuration);

            // Override the current ILogger implementation to use Serilog
            services.AddLogging(configure => configure.AddSerilog());

            ConfigureMongoDb(services);
            AddMongoDbForAspIdentity<ApplicationUserEntity, ApplicationRoleEntity>(services, config);

            services.AddTransient<IDataCache, DataCacheDataRepository>(sp => {
                var logger = sp.GetService<ILogger<IDataCache>>();
                ISharedCacheDataRepository dataRepository = sp.GetService<ISharedCacheDataRepository>();
                return new DataCacheDataRepository(logger, dataRepository);
            });
            services.AddTransient<IImageService, ImageService>();

            services.AddTransient<IWordTxtImporter, WordTxtImporter>(xp => {
                return new WordTxtImporter(config.Import);
            });

            services.AddTransient<IFilmImporter, FilmImporter>(xp => {
                return new FilmImporter(config.Import);
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

            services.AddTransient<SeedServiceDevelopmentLocalhost>();
            services.AddTransient<SeedServiceDevelopmentHeroku>();
            services.AddTransient<SeedServiceProductionHeroku>();

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

            return services.BuildServiceProvider();
        }

        private static Serilog.ILogger GetSerilogLogger(IConfiguration configuration, ApplicationSettings config)
        {
            var logger = new LoggerConfiguration()
                .Enrich.WithCorrelationId() // adds CorrelationId to log event. Ensure AddHttpContextAccessor is called.
                .Enrich.WithProperty("ApplicationContext", _appName) //define the context in logged data
                .Enrich.WithProperty("ApplicationEnvironment", HostingEnvironmentHelper.Environment) //define the environment
                .Enrich.FromLogContext() //allows to use specific context if nessesary
                .ReadFrom.Configuration(configuration);

            //if (HostingEnvironmentHelper.IsDevelopmentLocalhost())
            //{
            //    // write to file for development purposes
            //    logger.WriteTo.File(
            //        path: Path.Combine("./serilog-logs/local-logs.txt"),
            //        fileSizeLimitBytes: 100 * 1024 * 1024, // 100mb
            //                                               //restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
            //        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
            //        formatter: new Serilog.Sinks.Http.TextFormatters.NormalTextFormatter()
            //    );
            //}

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
            // no need. Just cast IDataRepository to IMongoDataRepository if needed

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

        private static void AddMongoDbForAspIdentity<TIdentity, TRole>(IServiceCollection services, ApplicationSettings config)
            where TIdentity : ApplicationUserEntity, new()
            where TRole : ApplicationRoleEntity, new()
        {
            // AspNetCore.Identity.MongoDbCore by Alexandre Spieser (allows to set custom Ids)
            // https://github.com/alexandre-spieser/AspNetCore.Identity.MongoDbCore
            IMongoDbContext mongoDbContext = new MongoDbContext(config.MongoDb.ConnectionString, config.MongoDb.DatabaseIdentity);
            //builder.Services.AddSingleton<IUserStore<TIdentity>>(x =>
            //{
            //    return new AspNetCore.Identity.MongoDbCore.MongoUserStore<TIdentity>(mongoDbContext);
            //});

            //builder.Services.AddSingleton<IRoleStore<TRole>>(x =>
            //{
            //    return new AspNetCore.Identity.MongoDbCore.MongoRoleStore<TRole>(mongoDbContext);
            //});

            services.AddIdentity<TIdentity, TRole>()
                .AddMongoDbStores<TIdentity, TRole, string>(mongoDbContext)
                .AddDefaultTokenProviders();
        }
    }
}
