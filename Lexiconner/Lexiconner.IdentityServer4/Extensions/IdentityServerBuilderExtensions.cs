using IdentityServer4.Services;
using IdentityServer4.Stores;
using Lexiconner.Application.Extensions;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.IdentityServer4.Store;
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbGenericRepository;
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
            builder.Services.AddSingleton<IIdentityServerConfig, IdentityServerConfig>();
            return builder;
        }

        /// <summary>
        /// Adds mongo repository (mongodb) for IdentityServer
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddMongoRepository(this IIdentityServerBuilder builder)
        {
            // register repository if wasn't registred yet
            if (!builder.Services.Any(x => x.ServiceType == typeof(IMongoRepository) && x.ImplementationType == typeof(MongoRepository)))
            {
                builder.Services.AddTransient<IMongoRepository, MongoRepository>(sp =>
                {
                    var config = sp.GetService<IOptions<ApplicationSettings>>().Value;
                    var mongoClient = sp.GetService<MongoClient>();
                    return new MongoRepository(mongoClient, config.MongoDb.Database, ApplicationDb.Identity);
                });
            }

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
            //where TIdentity : Microsoft.AspNetCore.Identity.MongoDB.IdentityUser 
            //where TRole     : Microsoft.AspNetCore.Identity.MongoDB.IdentityRole
            where TIdentity : ApplicationUserEntity, new ()
            where TRole : ApplicationRoleEntity, new()
        {

            //User Mongodb for Asp.net identity in order to get users stored
            var client = new MongoClient(config.MongoDb.ConnectionString);
            var database = client.GetDatabase(config.MongoDb.Database);

            //// Configure Asp Net Core Identity / Role to use MongoDB
            
            // Contrib.Microsoft.AspNetCore.Identity.MongoDB by thrixton (uses Mongo ObjectId)
            // https://github.com/thrixton/aspnetcore-identity-mongodb-netcore2plus
            //builder.Services.AddSingleton<IUserStore<TIdentity>>(x =>
            //{
            //    var usersCollection = database.GetCollection<TIdentity>(MongoConfig.GetCollectionName<TIdentity>());
            //    IndexChecks.EnsureUniqueIndexOnNormalizedEmail(usersCollection);
            //    IndexChecks.EnsureUniqueIndexOnNormalizedUserName(usersCollection);
            //    return new UserStore<TIdentity>(usersCollection);
            //});

            //builder.Services.AddSingleton<IRoleStore<TRole>>(x =>
            //{
            //    var rolesCollection = database.GetCollection<TRole>(MongoConfig.GetCollectionName<TRole>());
            //    IndexChecks.EnsureUniqueIndexOnNormalizedRoleName(rolesCollection);
            //    return new RoleStore<TRole>(rolesCollection);
            //});

            //builder.Services.AddIdentity<TIdentity, TRole>()
            //    .AddDefaultTokenProviders();


            // AspNetCore.Identity.MongoDbCore by Alexandre Spieser (allows to set custom Ids)
            // https://github.com/alexandre-spieser/AspNetCore.Identity.MongoDbCore
            IMongoDbContext mongoDbContext = new MongoDbContext(config.MongoDb.ConnectionString, config.MongoDb.Database);
            //builder.Services.AddSingleton<IUserStore<TIdentity>>(x =>
            //{
            //    return new AspNetCore.Identity.MongoDbCore.MongoUserStore<TIdentity>(mongoDbContext);
            //});

            //builder.Services.AddSingleton<IRoleStore<TRole>>(x =>
            //{
            //    return new AspNetCore.Identity.MongoDbCore.MongoRoleStore<TRole>(mongoDbContext);
            //});

            builder.Services.AddIdentity<TIdentity, TRole>()
                .AddMongoDbStores<ApplicationUserEntity, ApplicationRoleEntity, string>(mongoDbContext)
                .AddDefaultTokenProviders();

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
            if(cert == null && hostingEnvironment.IsDevelopmentAny())
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
