// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AspNetCore.Identity.MongoDbCore.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Lexiconner.Application.Helpers;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;

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
            };
        }

        public IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
            {
                new ApiScope("webapi", "Lexiconner Web Api", new List<string> {
                    //JwtClaimTypes.Name,
                    //JwtClaimTypes.GivenName,
                    //JwtClaimTypes.FamilyName,
                    //JwtClaimTypes.Email,
                    //JwtClaimTypes.WebSite,
                }),
                new ApiScope("browser_extension_webapi", "Lexiconner Browser Extension Web Api"),
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
            };

            // SPA client using code flow
            var spaClient = new Client
            {
                ClientId = "webspa",
                ClientName = "Lexiconner web SPA client",

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true, // Proof Key for Code Exchange (PKCE)
                RequireClientSecret = false,
                RequireConsent = false,

                RedirectUris =
                    {
                        $"{config.Urls.WebSpaExternalUrl}",
                        $"{config.Urls.WebSpaExternalUrl}/index.html",
                        $"{config.Urls.WebSpaExternalUrl}/callback.html",
                        $"{config.Urls.WebSpaExternalUrl}/silent.html",
                        $"{config.Urls.WebSpaExternalUrl}/popup.html",
                    },

                PostLogoutRedirectUris = {
                   $"{config.Urls.WebSpaExternalUrl}/index.html"
                },
                AllowedCorsOrigins = {
                   $"{config.Urls.WebSpaExternalUrl}"
                },

                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess, // refresh token
                    "webapi"
                },

                // identity token settigs
                IdentityTokenLifetime = Convert.ToInt32((new TimeSpan(0, 24, 0, 0)).TotalSeconds), // same as acess

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
            };

            // add local url for dev, because we use 1 db for dev and localhost for now,
            // but urls are different
            if (HostingEnvironmentHelper.IsDevelopmentAny())
            {
                spaClient.RedirectUris = spaClient.RedirectUris.Concat(new List<string>() {
                    $"{config.Urls.WebSpaLocalUrl}",
                    $"{config.Urls.WebSpaLocalUrl}/index.html",
                    $"{config.Urls.WebSpaLocalUrl}/callback.html",
                    $"{config.Urls.WebSpaLocalUrl}/silent.html",
                    $"{config.Urls.WebSpaLocalUrl}/popup.html",
                }).ToList();

                spaClient.PostLogoutRedirectUris = spaClient.PostLogoutRedirectUris.Concat(new List<string>() {
                    $"{config.Urls.WebSpaLocalUrl}",
                    $"{config.Urls.WebSpaLocalUrl}/index.html",
                }).ToList();

                spaClient.AllowedCorsOrigins = spaClient.AllowedCorsOrigins.Concat(new List<string>() {
                    $"{config.Urls.WebSpaLocalUrl}"
                }).ToList();
            }

            clients.Add(spaClient);

            // Browser extension SPA client using code flow
            var broswerExtensionSpaClient = new Client
            {
                ClientId = "browserExtension",
                ClientName = "Lexiconner browser extension client",

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true, // Proof Key for Code Exchange (PKCE)
                RequireClientSecret = false,
                RequireConsent = false,

                RedirectUris =
                    {
                        $"{config.Urls.ChromeExtensionUrl}",
                        $"{config.Urls.ChromeExtensionUrl}/index.html",
                        $"{config.Urls.ChromeExtensionUrl}/popup.html",
                        $"{config.Urls.ChromeExtensionUrl}/options.html",
                    },

                PostLogoutRedirectUris = {
                   $"{config.Urls.ChromeExtensionUrl}"
                },
                AllowedCorsOrigins = {
                   $"{config.Urls.ChromeExtensionUrl}"
                },

                AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess, // refresh token
                        "browser_extension_webapi"
                    },

                // identity token settigs
                IdentityTokenLifetime = Convert.ToInt32((new TimeSpan(0, 24, 0, 0)).TotalSeconds), // same as acess

                // access_token settings
                AccessTokenType = AccessTokenType.Jwt,
                AccessTokenLifetime = Convert.ToInt32((new TimeSpan(0, 24, 0, 0)).TotalSeconds),
                UpdateAccessTokenClaimsOnRefresh = true,

                // refresh token settings
                // refresh tokens are supported for the following flows: authorization code, hybrid and resource owner password credential flow.
                AllowOfflineAccess = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime = Convert.ToInt32((new TimeSpan(30, 0, 0, 0)).TotalSeconds),
                SlidingRefreshTokenLifetime = Convert.ToInt32((new TimeSpan(15, 0, 0, 0)).TotalSeconds),

                // send all claims in identity and access tokens
                AlwaysIncludeUserClaimsInIdToken = true,
                AlwaysSendClientClaims = true,
                ClientClaimsPrefix = "client_",
            };
            clients.Add(broswerExtensionSpaClient);

            if (HostingEnvironmentHelper.IsDevelopmentAny())
            {
                // SPA client using code flow
                clients.Add(new Client
                {
                    ClientId = "webtestspa",
                    ClientName = "Lexiconner web test SPA client",
                    ClientUri = config.Urls.WebTestSpaExternalUrl,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true, // Proof Key for Code Exchange (PKCE)
                    RequireClientSecret = false,
                    RequireConsent = false,

                    RedirectUris =
                    {
                        $"{config.Urls.WebTestSpaExternalUrl}",
                        $"{config.Urls.WebTestSpaExternalUrl}/index.html",
                        $"{config.Urls.WebTestSpaExternalUrl}/callback.html",
                        $"{config.Urls.WebTestSpaExternalUrl}/silent.html",
                        $"{config.Urls.WebTestSpaExternalUrl}/popup.html",
                    },
                    PostLogoutRedirectUris = {
                        $"{config.Urls.WebTestSpaExternalUrl}/index.html"
                    },
                    AllowedCorsOrigins = {
                        $"{config.Urls.WebTestSpaExternalUrl}"
                    },

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

                // IdentityServer v4 JavaScript Client
                clients.Add(new Client
                {
                    ClientId = "webtestspav4",
                    ClientName = "JavaScript Client v4",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        $"{config.Urls.WebTestSpaExternalUrl}",
                        $"{config.Urls.WebTestSpaExternalUrl}/index.html",
                        $"{config.Urls.WebTestSpaExternalUrl}/callback.html",
                        $"{config.Urls.WebTestSpaExternalUrl}/silent.html",
                        $"{config.Urls.WebTestSpaExternalUrl}/popup.html",
                    },
                    PostLogoutRedirectUris = {
                        $"{config.Urls.WebTestSpaExternalUrl}/index.html"
                    },
                    AllowedCorsOrigins = {
                        $"{config.Urls.WebTestSpaExternalUrl}"
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "webapi"
                    }
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
                    Name = RoleConfig.RootAdminRole
                },
                new ApplicationRoleEntity
                {
                    Name = RoleConfig.AdminRole
                },
                 new ApplicationRoleEntity
                {
                    Name = RoleConfig.UserRole
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
        //                RoleConfig.RootAdminRole,
        //                RoleConfig.AdminRole,
        //                RoleConfig.UserRole,
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
        //                RoleConfig.RootAdminRole,
        //                RoleConfig.AdminRole,
        //                RoleConfig.UserRole,
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
        //                RoleConfig.RootAdminRole,
        //                RoleConfig.AdminRole,
        //                RoleConfig.UserRole,
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
                    IsDemo = true,
                    Id = "604a1fc449aa2772d6a6674b",
                    Name = "John Doe",
                    UserName = "johndoe",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = "johndoe@test.com",
                    Roles = new List<string>
                    {
                        RoleConfig.UserRole,
                    },
                    Claims = new List<MongoClaim>
                    {
                        new MongoClaim() {
                            Type = JwtClaimTypes.Name,
                            Value = "John Doe",
                            Issuer = null,
                        },
                        new MongoClaim() {
                            Type = JwtClaimTypes.Email,
                            Value = "johndoe@test.com",
                            Issuer = null,
                        },
                        new MongoClaim() {
                            Type = JwtClaimTypes.EmailVerified,
                            Value = "true",
                            Issuer = null,
                        },
                    },
                    IsImportInitialData = true,
                    IsUpdateExistingDataOnSeed = true,
                },
                new ApplicationUserEntity
                {
                    IsDemo = true,
                    Id = "604a1fc1a2f958bcf8a11c21",
                    Name = "Bob Marley",
                    UserName = "bobmarley",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = "bobmarley@test.com",
                    Roles = new List<string>
                    {
                        RoleConfig.UserRole,
                    },
                    Claims = new List<MongoClaim>
                    {
                        new MongoClaim() {
                            Type = JwtClaimTypes.Name,
                            Value = "Bob Marley",
                            Issuer = null,
                        },
                        new MongoClaim() {
                            Type = JwtClaimTypes.Email,
                            Value = "bobmarley@test.com",
                            Issuer = null,
                        },
                        new MongoClaim() {
                            Type = JwtClaimTypes.EmailVerified,
                            Value = "true",
                            Issuer = null,
                        },
                    },
                    IsImportInitialData = true,
                    IsUpdateExistingDataOnSeed = true,
                },
                new ApplicationUserEntity
                {
                    IsDemo = false,
                    Id = "604a1fc96eb1dbf7894939d4",
                    Name = "Vadym Berkut",
                    UserName = "vadymberkut",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = "vadimberkut8@gmail.com",
                    Roles = new List<string>
                    {
                        RoleConfig.RootAdminRole,
                        RoleConfig.AdminRole,
                        RoleConfig.UserRole,
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
                        new MongoClaim() {
                            Type = JwtClaimTypes.EmailVerified,
                            Value = "true",
                            Issuer = null,
                        },
                    },
                    IsImportInitialData = true,
                    IsUpdateExistingDataOnSeed = true,
                },
                new ApplicationUserEntity
                {
                    IsDemo = false,
                    Id = "604a1fce40273f1a7ab652fc",
                    Name = "Bogdan Berkut",
                    UserName = "bogdanberkut",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = "bogdanberkut9@gmail.com",
                    Roles = new List<string>
                    {
                        RoleConfig.RootAdminRole,
                        RoleConfig.AdminRole,
                        RoleConfig.UserRole,
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
                        new MongoClaim() {
                            Type = JwtClaimTypes.EmailVerified,
                            Value = "true",
                            Issuer = null,
                        },
                    },
                    IsImportInitialData = true,
                    IsUpdateExistingDataOnSeed = true,
                },
            };
        }

        #endregion
    }
}