using System;
using System.Linq;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Lexiconner.IdentityServer4.Entities;
using Lexiconner.IdentityServer4.Exceptions;
using Lexiconner.IdentityServer4.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
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
                var identityServerConfig = app.ApplicationServices.GetService<IdentityServerConfig>();
                var repository = app.ApplicationServices.GetService<IMongoRepository>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();

                // Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
                ConfigureMongoDriver2IgnoreExtraElements();

                // Client
                if (!repository.CollectionExists<Client>())
                {
                    foreach (var client in identityServerConfig.GetClients())
                    {
                        if(!repository.Exists<Client>(x => x.ClientId == client.ClientId))
                        {
                            repository.Add(client);
                        }
                    }
                }

                // IdentityResource
                if (!repository.CollectionExists<IdentityResource>())
                {
                    foreach (var res in identityServerConfig.GetIdentityResources())
                    {
                        if (!repository.Exists<IdentityResource>(x => x.Name == res.Name))
                        {
                            repository.Add(res);
                        }
                    }
                }

                // ApiResource
                if (!repository.CollectionExists<ApiResource>())
                {
                    foreach (var api in identityServerConfig.GetApiResources())
                    {
                        if (!repository.Exists<ApiResource>(x => x.Name == api.Name))
                        {
                            repository.Add(api);
                        }
                    }
                }

                if(hostingEnvironment.IsDevelopment())
                {
                    repository.DeleteAllAsync<ApplicationRole>().GetAwaiter().GetResult();
                    repository.DeleteAllAsync<ApplicationUser>().GetAwaiter().GetResult();
                }
                if (!repository.CollectionExists<ApplicationRole>())
                {
                    AddInitialRoles(identityServerConfig, roleManager);
                }
                if (!repository.CollectionExists<ApplicationUser>())
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

        /// <summary>
        /// Populate MongoDB with a List of Dummy users to enable tests - e.g. Alice, Bob
        ///   see Config.GetSampleUsers() for details.
        /// </summary>
        /// <param name="userManager"></param>
        private static void AddSampleUsers(IdentityServerConfig identityServerConfig, UserManager<ApplicationUser> userManager)
        {
            var dummyUsers = identityServerConfig.GetSampleIdentityServerUsers();

            foreach (var usrDummy in dummyUsers)
            {
                var userDummyEmail = usrDummy.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email);

                if (userDummyEmail == null)
                {
                    throw new Exception("Could not locate user email from claims!");
                }

                var existing = userManager.FindByEmailAsync(userDummyEmail.Value).GetAwaiter().GetResult();
                if(existing != null)
                {
                    userManager.DeleteAsync(existing).GetAwaiter().GetResult();
                }

                var user = new ApplicationUser()
                {
                    UserName = usrDummy.Username,
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = userDummyEmail.Value,
                    NormalizedEmail = userDummyEmail.Value
                };

                foreach (var claim in usrDummy.Claims)
                {
                    user.AddClaim(claim);
                }
                var result = userManager.CreateAsync(user, usrDummy.Password);
                if (!result.Result.Succeeded)
                {
                    var errorList = result.Result.Errors.ToList();
                    throw new Exception(string.Join("; ", errorList));
                }
            }
        }

        private static void AddInitialRoles(IdentityServerConfig identityServerConfig, RoleManager<ApplicationRole> roleManager)
        {
            var roles = identityServerConfig.GetInitialIdentityRoles();

            foreach (var role in roles)
            {
                var existing = roleManager.FindByNameAsync(role.Name).GetAwaiter().GetResult();
                if (existing != null)
                {
                    roleManager.DeleteAsync(existing).GetAwaiter().GetResult();
                }

                var result = roleManager.CreateAsync(role);
                if (!result.Result.Succeeded)
                {
                    var errorList = result.Result.Errors.ToList();
                    throw new Exception(string.Join("; ", errorList));
                }
            }
        }

        private static void AddInitialUsers(IdentityServerConfig identityServerConfig, UserManager<ApplicationUser> userManager)
        {
            var users = identityServerConfig.GetInitialdentityUsers();

            foreach (var user in users)
            {
                var existing = userManager.FindByEmailAsync(user.Email).GetAwaiter().GetResult();
                if (existing != null)
                {
                    userManager.DeleteAsync(existing).GetAwaiter().GetResult();
                }

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
