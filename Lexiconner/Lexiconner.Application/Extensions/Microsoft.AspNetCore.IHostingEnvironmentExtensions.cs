using Lexiconner.Application.Helpers;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Lexiconner.Application.Extensions
{
    public static partial class IWebHostEnvironmentExtensions
    {
        public static bool IsDevelopmentLocalhost(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentLocalhost(GetEnvironment());
        }

        public static bool IsDevelopmentDocker(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentDocker(GetEnvironment());
        }

        public static bool IsDevelopmentAzure(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentAzure(GetEnvironment());
        }

        public static bool IsDevelopmentHeroku(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentHeroku(GetEnvironment());
        }

        public static bool IsDevelopmentLocalhostOrDocker(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentLocalhost(GetEnvironment()) || HostingEnvironmentHelper.IsDevelopmentDocker(GetEnvironment());
        }

        /// <summary>
        /// Returns true if current environment is Development or any custom type like DevelopmentLocalhost, DevelopmentDocker, Development[anything]
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <returns></returns>
        public static bool IsDevelopmentAny(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentAny(GetEnvironment());
        }

        public static bool IsTestingAny(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsTestingAny(GetEnvironment());
        }

        public static bool IsProductionAzure(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsProductionAzure(GetEnvironment());
        }

        public static bool IsProductionHeroku(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsProductionHeroku(GetEnvironment());
        }

        public static bool IsProductionAny(this IWebHostEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsProductionAny(GetEnvironment());
        }

        private static string GetEnvironment()
        {
            return HostingEnvironmentHelper.Environment;
        }
    }
}