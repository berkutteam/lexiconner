using System;
using System.Linq;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Lexiconner.IdentityServer4.Entities;
using Lexiconner.IdentityServer4.Exceptions;
using Lexiconner.IdentityServer4.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Lexiconner.IdentityServer4.Extensions
{
    public static class MongoDbStartup
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
                var identityServerConfig = app.ApplicationServices.GetService<IdentityServerConfig>();
                var repository = app.ApplicationServices.GetService<IRepository>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                // Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
                ConfigureMongoDriver2IgnoreExtraElements();

                var createdNewRepository = false;

                // Client
                if (!repository.CollectionExists<Client>())
                {
                    foreach (var client in identityServerConfig.GetClients())
                    {
                        repository.Add(client);
                    }
                    createdNewRepository = true;
                }

                // IdentityResource
                if (!repository.CollectionExists<IdentityResource>())
                {
                    foreach (var res in identityServerConfig.GetIdentityResources())
                    {
                        repository.Add(res);
                    }
                    createdNewRepository = true;
                }

                // ApiResource
                if (!repository.CollectionExists<ApiResource>())
                {
                    foreach (var api in identityServerConfig.GetApiResources())
                    {
                        repository.Add(api);
                    }
                    createdNewRepository = true;
                }

                // Populate MongoDB with dummy users to enable test - e.g. Bob, Alice
                if (createdNewRepository)
                {
                    AddSampleUsersToMongo(identityServerConfig, userManager);
                }

                // If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
                if (createdNewRepository)
                {
                    throw new HostRestartRequiredException(_newRepositoryMsg);
                }
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
        private static void AddSampleUsersToMongo(IdentityServerConfig identityServerConfig, UserManager<ApplicationUser> userManager)
        {
            var dummyUsers = identityServerConfig.GetSampleUsers();

            foreach (var usrDummy in dummyUsers)
            {
                var userDummyEmail = usrDummy.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email);

                if (userDummyEmail == null)
                {
                    throw new Exception("Could not locate user email from claims!");
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
                    // If we got an error, Make sure to drop all collections from Mongo before trying again. Otherwise sample users will NOT be populated
                    var errorList = result.Result.Errors.ToArray();
                    throw new Exception($"Error Adding sample users to MongoDB! Make sure to drop all collections from Mongo before trying again!");
                }
            }
        }
    }
}
