// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AspNetCore.Identity.MongoDbCore.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.Helpers;
using Lexiconner.Domain.Entitites;
using Lexiconner.IdentityServer4.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Lexiconner.IdentityServer4.Config
{
    public class IdentityServerConfig : IIdentityServerConfig
    {
        public IdentityServerConfig(
        )
        {
        }

        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResources.Address(),
            };
        }

        public IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource("webapi", "Lexiconner Web Api", new List<string> {
                    //JwtClaimTypes.Name,
                    //JwtClaimTypes.GivenName,
                    //JwtClaimTypes.FamilyName,
                    //JwtClaimTypes.Email,
                    //JwtClaimTypes.WebSite,
                })
            };
        }

        public IEnumerable<Client> GetClients(ApplicationSettings config)
        {
            var clients = new List<Client>
            {
                // client credentials flow client
                //new Client
                //{
                //    ClientId = "client",
                //    ClientName = "Client Credentials Client",

                //    AllowedGrantTypes = GrantTypes.ClientCredentials,
                //    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                //    AllowedScopes = { "api1" }
                //},

                // MVC client using hybrid flow
                //new Client
                //{
                //    ClientId = "mvc",
                //    ClientName = "MVC Client",

                //    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                //    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                //    RedirectUris = { "http://localhost:5001/signin-oidc" },
                //    FrontChannelLogoutUri = "http://localhost:5001/signout-oidc",
                //    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },

                //    AllowOfflineAccess = true,
                //    AllowedScopes = { "openid", "profile", "api1" }
                //},

                // SPA client using code flow
                new Client
                {
                    ClientId = "webspa",
                    ClientName = "Lexiconner Web SPA Client",
                    ClientUri = config.Urls.WebSpa,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true, // Proof Key for Code Exchange (PKCE)
                    RequireClientSecret = false,
                    RequireConsent = false,

                    RedirectUris =
                    {
                        $"{config.Urls.WebSpa}",
                        $"{config.Urls.WebSpa}/index.html",
                        $"{config.Urls.WebSpa}/callback.html",
                        $"{config.Urls.WebSpa}/silent.html",
                        $"{config.Urls.WebSpa}/popup.html",
                    },

                    PostLogoutRedirectUris = { $"{config.Urls.WebSpa}/index.html" },
                    AllowedCorsOrigins = { $"{config.Urls.WebSpa}" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess, // refresh token
                        "webapi"
                    },

                    // access_token settings
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = Convert.ToInt32((new TimeSpan(7, 0, 0, 0)).TotalSeconds),
                    UpdateAccessTokenClaimsOnRefresh = true,

                    // refresh token settings
                    // refresh tokens are supported for the following flows: authorization code, hybrid and resource owner password credential flow.
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = Convert.ToInt32((new TimeSpan(30, 0, 0, 0)).TotalSeconds),
                    SlidingRefreshTokenLifetime = Convert.ToInt32((new TimeSpan(15, 0, 0, 0)).TotalSeconds),
                },
            };

            if(HostingEnvironmentHelper.IsDevelopmentAny())
            {
                // SPA client using code flow
                clients.Add(new Client
                {
                    ClientId = "webtestspa",
                    ClientName = "Lexiconner Web Test SPA Client",
                    ClientUri = config.Urls.WebTestSpa,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true, // Proof Key for Code Exchange (PKCE)
                    RequireClientSecret = false,
                    RequireConsent = false,

                    RedirectUris =
                    {
                        $"{config.Urls.WebTestSpa}",
                        $"{config.Urls.WebTestSpa}/index.html",
                        $"{config.Urls.WebTestSpa}/callback.html",
                        $"{config.Urls.WebTestSpa}/silent.html",
                        $"{config.Urls.WebTestSpa}/popup.html",
                    },

                    PostLogoutRedirectUris = { $"{config.Urls.WebTestSpa}/index.html" },
                    AllowedCorsOrigins = { $"{config.Urls.WebTestSpa}" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess, // refresh token
                        "webapi"
                    },

                    // refresh token settings
                    // refresh tokens are supported for the following flows: authorization code, hybrid and resource owner password credential flow.
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = Convert.ToInt32((new TimeSpan(30, 0, 0, 0)).TotalSeconds),
                    SlidingRefreshTokenLifetime = Convert.ToInt32((new TimeSpan(15, 0, 0, 0)).TotalSeconds),

                    //Claims = new List<Claim>
                    //{
                    //    new Claim("test-set-in-client-config", "test")
                    //},
                });
            }

            return clients;
        }

        public string DefaultUserPassword { get { return "Password_1"; } }

        public List<ApplicationRoleEntity> GetInitialIdentityRoles()
        {
            return new List<ApplicationRoleEntity>
            {
                new ApplicationRoleEntity
                {
                    Name = "RootAdmin"
                },
                new ApplicationRoleEntity
                {
                    Name = "Admin"
                },
                 new ApplicationRoleEntity
                {
                    Name = "User"
                }
            };
        }

        #region Contrib.Microsoft.AspNetCore.Identity.MongoDB by thrixton (uses Mongo ObjectId)

        //public List<ApplicationUserEntity> GetInitialdentityUsers()
        //{
        //    return new List<ApplicationUserEntity>
        //    {
        //        new ApplicationUserEntity
        //        {
        //            //Id = ,
        //            Name = "John Doe1",
        //            UserName = "johndoe1",
        //            LockoutEnabled = false,
        //            EmailConfirmed = true,
        //            Email = "johndoe1@test.com",
        //            Roles = new List<string>
        //            {
        //                "RootAdmin",
        //                "Admin",
        //                "User",
        //            },
        //            Claims = new List<IdentityUserClaim>
        //            {
        //                new IdentityUserClaim(new Claim(JwtClaimTypes.Name, "John Doe1")),
        //                new IdentityUserClaim(new Claim(JwtClaimTypes.Email, "johndoe1@gmail.com")),
        //            },
        //            IsImportInitialData = true
        //        },
        //        new ApplicationUserEntity
        //        {
        //            //Id = ,
        //            Name = "Vadym Berkut",
        //            UserName = "vadymberkut",
        //            LockoutEnabled = false,
        //            EmailConfirmed = true,
        //            Email = "vadimberkut8@gmail.com",
        //            Roles = new List<string>
        //            {
        //                "RootAdmin",
        //                "Admin",
        //                "User",
        //            },
        //            Claims = new List<IdentityUserClaim>
        //            {
        //                new IdentityUserClaim(new Claim(JwtClaimTypes.Name, "Vadym Berkut")),
        //                new IdentityUserClaim(new Claim(JwtClaimTypes.Email, "vadimberkut8@gmail.com")),
        //            },
        //            IsImportInitialData = true
        //        },
        //        new ApplicationUserEntity
        //        {
        //            //Id = ,
        //            Name = "Bogdan Berkut",
        //            UserName = "bogdanberkut",
        //            LockoutEnabled = false,
        //            EmailConfirmed = true,
        //            Email = "bogdanberkut9@gmail.com",
        //            Roles = new List<string>
        //            {
        //                "RootAdmin",
        //                "Admin",
        //                "User",
        //            },
        //            Claims = new List<IdentityUserClaim>
        //            {
        //                new IdentityUserClaim(new Claim(JwtClaimTypes.Name, "Bogdan Berkut")),
        //                new IdentityUserClaim(new Claim(JwtClaimTypes.Email, "bogdanberkut9@gmail.com")),
        //            },
        //            IsImportInitialData = true
        //        },
        //    };
        //}

        #endregion

        #region AspNetCore.Identity.MongoDbCore by Alexandre Spieser (allows to set custom Ids)

        public List<ApplicationUserEntity> GetInitialdentityUsers()
        {
            return new List<ApplicationUserEntity>
            {
                new ApplicationUserEntity
                {
                    Id = "01DGMH8EQAZS9M4VS6X5FB2HDA",
                    Name = "John Doe1",
                    UserName = "johndoe1",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = "johndoe1@test.com",
                    Roles = new List<string>
                    {
                        "RootAdmin",
                        "Admin",
                        "User",
                    },
                    Claims = new List<MongoClaim>
                    {
                        new MongoClaim() {
                            Type = JwtClaimTypes.Name,
                            Value = "John Doe1",
                            Issuer = null,
                        },
                        new MongoClaim() {
                            Type = JwtClaimTypes.Email,
                            Value = "johndoe1@gmail.com",
                            Issuer = null,
                        },
                    },
                    IsImportInitialData = true
                },
                new ApplicationUserEntity
                {
                    Id = "01DGMH8EQC7V8N3R6RHQYY2VH7",
                    Name = "Vadym Berkut",
                    UserName = "vadymberkut",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = "vadimberkut8@gmail.com",
                    Roles = new List<string>
                    {
                        "RootAdmin",
                        "Admin",
                        "User",
                    },
                    Claims = new List<MongoClaim>
                    {
                        new MongoClaim() {
                            Type = JwtClaimTypes.Name,
                            Value = "Vadym Berkut",
                            Issuer = null,
                        },
                        new MongoClaim() {
                            Type = JwtClaimTypes.Email,
                            Value = "vadimberkut8@gmail.com",
                            Issuer = null,
                        },
                    },
                    IsImportInitialData = true
                },
                new ApplicationUserEntity
                {
                    Id = "01DGMH8EQC2B6F26KAPF9PRW59",
                    Name = "Bogdan Berkut",
                    UserName = "bogdanberkut",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = "bogdanberkut9@gmail.com",
                    Roles = new List<string>
                    {
                        "RootAdmin",
                        "Admin",
                        "User",
                    },
                    Claims = new List<MongoClaim>
                    {
                        new MongoClaim() {
                            Type = JwtClaimTypes.Name,
                            Value = "Bogdan Berkut",
                            Issuer = null,
                        },
                        new MongoClaim() {
                            Type = JwtClaimTypes.Email,
                            Value = "bogdanberkut9@gmail.com",
                            Issuer = null,
                        },
                    },
                    IsImportInitialData = true
                },
            };
        }

        #endregion
    }
}