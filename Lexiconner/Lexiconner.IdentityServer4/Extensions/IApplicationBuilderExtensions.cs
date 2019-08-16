using System;
using System.Linq;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Lexiconner.Application.Extensions;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.IdentityModel;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.IdentityServer4.Exceptions;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Lexiconner.IdentityServer4.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        private static string _newRepositoryMsg = $"Mongo Repository created/populated! Please restart your website, so Mongo driver will be configured  to ignore Extra Elements.";

        /// <summary>
        /// Adds the support for MongoDb Persistance for all identityserver stored
        /// </summary>
        /// <remarks><![CDATA[
        /// It implements and used mongodb collections for:
        /// - Clients
        /// - IdentityResources
        /// - ApiResource
        /// ]]></remarks>
        public static void UseMongoDbForIdentityServer(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var hostingEnvironment = app.ApplicationServices.GetService<IHostingEnvironment>();
                var identityServerConfig = app.ApplicationServices.GetService<IIdentityServerConfig>();
                var dataRepository = app.ApplicationServices.GetService<IDataRepository>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUserEntity>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<ApplicationRoleEntity>>();

                // data initialization moved to Seed
            }
        }
    }
}
