﻿using IdentityServer4.Services;
using IdentityServer4.Stores;
using Lexiconner.IdentityServer4.Repository;
using Lexiconner.IdentityServer4.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Adds IdentityServerConfig to DI
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddConfig(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IdentityServerConfig>();
            return builder;
        }

        /// <summary>
        /// Adds mongo repository (mongodb) for IdentityServer
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddMongoRepository(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IMongoRepository, MongoRepository>();
            return builder;
        }

        /// <summary>
        /// Adds mongodb implementation for the "Asp Net Core Identity" part (saving user and roles)
        /// </summary>
        /// <remarks><![CDATA[
        /// Contains implemenations for
        /// - IUserStore<T>
        /// - IRoleStore<T>
        /// ]]></remarks>
        public static IIdentityServerBuilder AddMongoDbForAspIdentity<TIdentity, TRole>(this IIdentityServerBuilder builder, ApplicationSettings config) 
            where TIdentity : Microsoft.AspNetCore.Identity.MongoDB.IdentityUser 
            where TRole     : Microsoft.AspNetCore.Identity.MongoDB.IdentityRole
        {

            //User Mongodb for Asp.net identity in order to get users stored
            var client = new MongoClient(config.MongoDb.ConnectionString);
            var database = client.GetDatabase(config.MongoDb.Database);

            // Configure Asp Net Core Identity / Role to use MongoDB
            builder.Services.AddSingleton<IUserStore<TIdentity>>(x =>
            {
                var usersCollection = database.GetCollection<TIdentity>(MongoConfig.GetCollectionName<TIdentity>());
                IndexChecks.EnsureUniqueIndexOnNormalizedEmail(usersCollection);
                IndexChecks.EnsureUniqueIndexOnNormalizedUserName(usersCollection);
                return new UserStore<TIdentity>(usersCollection);
            });

            builder.Services.AddSingleton<IRoleStore<TRole>>(x =>
            {
                var rolesCollection = database.GetCollection<TRole>(MongoConfig.GetCollectionName<TRole>());
                IndexChecks.EnsureUniqueIndexOnNormalizedRoleName(rolesCollection);
                return new RoleStore<TRole>(rolesCollection);
            });
            builder.Services.AddIdentity<TIdentity, TRole>();

            return builder;
        }

        /// <summary>
        /// Configure ClientId / Secrets
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurationOption"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddClients(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, CustomClientStore>();
            builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();
            return builder;
        }


        /// <summary>
        /// Configure API  &  Resources
        /// Note: Api's have also to be configured for clients as part of allowed scope for a given clientID 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddIdentityApiResources(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IResourceStore, CustomResourceStore>();
            return builder;
        }

        /// <summary>
        /// Configure Grants
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddPersistedGrants(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IPersistedGrantStore, CustomPersistedGrantStore>();
            return builder;
        }

        /// <summary>
        /// Configures signing credentials
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddSigningCredentialCustom(this IIdentityServerBuilder builder, IHostingEnvironment hostingEnvironment, ApplicationSettings config)
        {
            X509Certificate2 cert = null;

            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);

                // we use the My/Personal store of the CurrentUser registry. 
                // this is where Azure will load the certificate when we upload it later.
                var certCollection = certStore.Certificates.Find(X509FindType.FindByIssuerName, config.IdenitytServer4.SigningCredential.KeyStoreIssuer, false);

                if (certCollection.Count > 0)
                {
                    cert = certCollection[0];
                    builder.AddSigningCredential(cert);
                    Log.Logger.Information($"Successfully loaded cert from registry: {cert.IssuerName.Name} / {cert.Thumbprint}");
                }
            }

            // fallback to local file
            if (cert == null)
            {
                var path = Path.Combine(hostingEnvironment.ContentRootPath, config.IdenitytServer4.SigningCredential.KeyFilePath);
                if(File.Exists(path))
                {
                    cert = new X509Certificate2(path, config.IdenitytServer4.SigningCredential.KeyFilePassword);

                    // check certificate works
                    // should output: System.Security.Cryptography.RSACng
                    // otherwise exception will be thrown
                    Log.Logger.Information($"Certificate loaded: {cert.PrivateKey.ToString()}");

                    builder.AddSigningCredential(cert);
                    Log.Logger.Information($"Falling back to cert from file. Successfully loaded: {cert.IssuerName.Name} / {cert.Thumbprint}");
                }
            }

            // fallback to generated developer local file for development
            if(cert == null && hostingEnvironment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential(
                    persistKey: true,
                    filename: Path.Combine(hostingEnvironment.ContentRootPath, config.IdenitytServer4.SigningCredential.KeyFilePathDeveloper)
                );
            }
            else if(cert == null)
            {
                throw new InvalidOperationException($"{nameof(AddSigningCredentialCustom)}: Can't find certificate both in store and file.");
            }

            return builder;
        }
    }
}