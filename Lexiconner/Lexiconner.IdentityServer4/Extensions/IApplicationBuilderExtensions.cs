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

                // Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
                ConfigureMongoDriver2IgnoreExtraElements();

                // Client
                foreach (var client in identityServerConfig.GetClients())
                {
                    if(!dataRepository.ExistsAsync<ClientEntity>(x => x.Client.ClientId == client.ClientId).GetAwaiter().GetResult())
                    {
                        dataRepository.AddAsync<ClientEntity>(new ClientEntity(client)).GetAwaiter().GetResult();
                    }
                }

                // IdentityResource
                foreach (var res in identityServerConfig.GetIdentityResources())
                {
                    if (!dataRepository.ExistsAsync<IdentityResourceEntity>(x => x.IdentityResource.Name == res.Name).GetAwaiter().GetResult())
                    {
                        dataRepository.AddAsync(new IdentityResourceEntity(res)).GetAwaiter().GetResult();
                    }
                }

                // ApiResource
                foreach (var api in identityServerConfig.GetApiResources())
                {
                    if (!dataRepository.ExistsAsync<ApiResourceEntity>(x => x.ApiResource.Name == api.Name).GetAwaiter().GetResult())
                    {
                        dataRepository.AddAsync(new ApiResourceEntity(api)).GetAwaiter().GetResult();
                    }
                }

                if(hostingEnvironment.IsDevelopmentLocalhost())
                {
                    //repository.DeleteAllAsync<ApplicationRoleEntity>().GetAwaiter().GetResult();
                    //repository.DeleteAllAsync<ApplicationUserEntity>().GetAwaiter().GetResult();
                }
                if (!dataRepository.CollectionExistsAsync<ApplicationRoleEntity>().GetAwaiter().GetResult())
                {
                    AddInitialRoles(identityServerConfig, roleManager);
                }
                if (!dataRepository.CollectionExistsAsync<ApplicationUserEntity>().GetAwaiter().GetResult())
                {
                    AddInitialUsers(identityServerConfig, userManager);
                }

                // If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
                //if (createdNewRepository)
                //{
                //    throw new HostRestartRequiredException(_newRepositoryMsg);
                //}
            }
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            if(!BsonClassMap.IsClassMapRegistered(typeof(Client))) {
                BsonClassMap.RegisterClassMap<Client>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(IdentityResource)))
            {
                BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(ApiResource)))
            {
                BsonClassMap.RegisterClassMap<ApiResource>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(PersistedGrant)))
            {
                BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

        private static void AddInitialRoles(IIdentityServerConfig identityServerConfig, RoleManager<ApplicationRoleEntity> roleManager)
        {
            var roles = identityServerConfig.GetInitialIdentityRoles();

            foreach (var role in roles)
            {
                var existing = roleManager.FindByNameAsync(role.Name).GetAwaiter().GetResult();
                if(existing == null)
                {
                    var result = roleManager.CreateAsync(role);
                    if (!result.Result.Succeeded)
                    {
                        var errorList = result.Result.Errors.ToList();
                        throw new Exception(string.Join("; ", errorList));
                    }
                }
            }
        }

        private static void AddInitialUsers(IIdentityServerConfig identityServerConfig, UserManager<ApplicationUserEntity> userManager)
        {
            var users = identityServerConfig.GetInitialdentityUsers();

            foreach (var user in users)
            {
                var existing = userManager.FindByEmailAsync(user.Email).GetAwaiter().GetResult();
                if (existing == null)
                {
                    var result = userManager.CreateAsync(user, "Password_1");
                    if (!result.Result.Succeeded)
                    {
                        var errorList = result.Result.Errors.ToList();
                        throw new Exception(string.Join("; ", errorList));
                    }
                }
            }
        }
    }
}
