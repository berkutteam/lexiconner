using Lexiconner.Application.Helpers;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Lexiconner.Application.Extensions
{
    public static partial class IHostingEnvironmentExtensions
    {
        public static bool IsDevelopmentLocalhost(this IHostingEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentLocalhost(GetEnvironment());
        }

        public static bool IsDevelopmentDocker(this IHostingEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentDocker(GetEnvironment());
        }

        public static bool IsDevelopmentLocalhostOrDocker(this IHostingEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentLocalhost(GetEnvironment()) || HostingEnvironmentHelper.IsDevelopmentDocker(GetEnvironment());
        }

        /// <summary>
        /// Returns true if current environment is Development or any custom type like DevelopmentLocalhost, DevelopmentDocker, Development[anything]
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <returns></returns>
        public static bool IsDevelopmentAny(this IHostingEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsDevelopmentAny(GetEnvironment());
        }

        public static bool IsTestingAny(this IHostingEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsTestingAny(GetEnvironment());
        }

        public static bool IsProductionAzure(this IHostingEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsProductionAzure(GetEnvironment());
        }

        public static bool IsProductionAny(this IHostingEnvironment hostingEnvironment)
        {
            return HostingEnvironmentHelper.IsProductionAny(GetEnvironment());
        }

        private static string GetEnvironment()
        {
            return HostingEnvironmentHelper.Environment;
        }
    }
}