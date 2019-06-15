using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lexiconner.Application.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lexiconner.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    // load env variables from .env file
                    string envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                    if (File.Exists(envFilePath))
                    {
                        DotNetEnv.Env.Load(envFilePath);
                    }

                    configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configBuilder.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    configBuilder.AddEnvironmentVariables();
                });

            if (HostingEnvironmentHelper.IsDevelopmentLocalhost())
            {
                builder.UseUrls($"http://localhost:5005;https://localhost:5006");
            }

            if (Environment.GetEnvironmentVariable("RUNTIME_ENV") == "heroku")
            {
                builder.UseUrls($"http://+:{Environment.GetEnvironmentVariable("PORT")}");
            }

            builder.UseStartup<Startup>();

            return builder;
        }
    }
}
