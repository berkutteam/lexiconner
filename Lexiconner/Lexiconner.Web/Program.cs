﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lexiconner.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    // load env variables from .env file
                    DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

                    configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configBuilder.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    configBuilder.AddEnvironmentVariables();
                })
                .UseStartup<Startup>();
    }
}
