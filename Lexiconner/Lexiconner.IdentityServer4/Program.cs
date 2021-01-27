// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Autofac.Extensions.DependencyInjection;
using Lexiconner.Application.Helpers;
using Lexiconner.IdentityServer4.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Lexiconner.IdentityServer4
{
    public class Program
    {
        // not changing unique app name.
        private static readonly string _appName = "Lexiconner.Identity";

        public static int Main(string[] args)
        {
            // check environment is set
            if (String.IsNullOrEmpty(HostingEnvironmentHelper.Environment))
            {
                Log.Error("ASPNETCORE_ENVIRONMENT env variable must be set!");
                return 1;
            }

            //here we need configuration to get the Serilog sinks properties
            //The AI key in this microservice
            var configuration = GetConfiguration();
            var config = configuration.Get<ApplicationSettings>();

            //here we congigure the Serilog. Nothing special all according documentation of Serilog
            Log.Logger = GetSerilogLogger(configuration, config);

            ShowEnvironmentInfo();

            //here before the ILogger configured and ovverided we use the Log static helper of Serilog
            Log.Information("Configuring web host ({ApplicationContext})...", _appName);
            var host = CreateHostBuilder(args).Build();

            Log.Information("Starting web host ({ApplicationContext})...", _appName);
            host.Run();

            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder
                        // UseConfiguration must be first to work properly
                        .UseConfiguration(GetConfiguration())

                        // set URLs directly, because it doesn't pick up ASPNETCORE_URLS when UseConfiguration is applied earlier
                        // UseUrls must follow UseConfiguration to work properly
                        .UseUrls(Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://+:80")
                        .UseStartup<Startup>();

                     if (HostingEnvironmentHelper.IsDevelopmentLocalhost())
                     {
                         webBuilder.UseUrls($"http://localhost:5004");
                     }

                     if (HostingEnvironmentHelper.IsHerokuAny())
                     {
                         webBuilder.UseUrls($"http://+:{Environment.GetEnvironmentVariable("PORT")}");
                     }
                 });
        }

        private static IConfiguration GetConfiguration()
        {
            // load env variables from .env file
            string envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (File.Exists(envFilePath))
            {
                DotNetEnv.Env.Load(envFilePath);
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{HostingEnvironmentHelper.Environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            return config;
        }

        private static Serilog.ILogger GetSerilogLogger(IConfiguration configuration, ApplicationSettings config)
        {
            var logger = new LoggerConfiguration()
                .Enrich.WithCorrelationId() // adds CorrelationId to log event. Ensure AddHttpContextAccessor is called.
                .Enrich.WithProperty("ApplicationContext", _appName) //define the context in logged data
                .Enrich.WithProperty("ApplicationEnvironment", HostingEnvironmentHelper.Environment) //define the environment
                .Enrich.FromLogContext() //allows to use specific context if nessesary
                .ReadFrom.Configuration(configuration);

            if (HostingEnvironmentHelper.IsDevelopmentLocalhost())
            {
                // write to file for development purposes
                logger.WriteTo.File(
                    path: Path.Combine("./serilog-logs/local-logs.txt"),
                    fileSizeLimitBytes: 100 * 1024 * 1024, // 100mb
                                                           //restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
                    formatter: new Serilog.Sinks.Http.TextFormatters.NormalTextFormatter()
                );
            }

            return logger.CreateLogger();
        }

        private static void ShowEnvironmentInfo()
        {
            Log.Information(Environment.NewLine);
            Log.Information("Environment info:");
            Log.Information("ASPNETCORE_ENVIRONMENT: {EnvironmentVariable}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            Log.Information("ASPNETCORE_URLS: {EnvironmentVariable}", Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));
            Log.Information("PORT: {PORT}", Environment.GetEnvironmentVariable("PORT"));
            Log.Information(Environment.NewLine);
        }
    }
}
