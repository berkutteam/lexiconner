// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;

namespace Lexiconner.IdentityServer4
{
    public class IdentityServerConfig
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ApplicationSettings _config;

        public IdentityServerConfig(IHostingEnvironment hostingEnvironment, IOptions<ApplicationSettings> config)
        {
            _config = config.Value;
            _hostingEnvironment = hostingEnvironment;
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

        public IEnumerable<Client> GetClients()
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

                // SPA client using implicit flow
                new Client
                {
                    ClientId = "webspa",
                    ClientName = "Lexiconner Web SPA Client",
                    ClientUri = _config.Urls.WebSpa,

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        $"{_config.Urls.WebSpa}/index.html",
                        $"{_config.Urls.WebSpa}/callback.html",
                        $"{_config.Urls.WebSpa}/silent.html",
                        $"{_config.Urls.WebSpa}/popup.html",
                    },

                    PostLogoutRedirectUris = { $"{_config.Urls.WebSpa}/index.html" },
                    AllowedCorsOrigins = { $"{_config.Urls.WebSpa}" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "webapi"
                    }
                },
            };

            if(_hostingEnvironment.IsDevelopment())
            {
                // SPA client using implicit flow
                clients.Add(new Client
                {
                    ClientId = "webtestspa",
                    ClientName = "Lexiconner Web Test SPA Client",
                    ClientUri = _config.Urls.WebTestSpa,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true, // Proof Key for Code Exchange (PKCE)
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        $"{_config.Urls.WebTestSpa}/index.html",
                        $"{_config.Urls.WebTestSpa}/callback.html",
                        $"{_config.Urls.WebTestSpa}/silent.html",
                        $"{_config.Urls.WebTestSpa}/popup.html",
                    },

                    PostLogoutRedirectUris = { $"{_config.Urls.WebTestSpa}/index.html" },
                    AllowedCorsOrigins = { $"{_config.Urls.WebTestSpa}" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "webapi"
                    },

                    //Claims = new List<Claim>
                    //{
                    //    new Claim("test-set-in-client-config", "test")
                    //},
                });
            }

            return clients;
        }

        public List<TestUser> GetSampleUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "818728",
                    Username = "alice",
                    Password = "Password_1",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "alice@test.com"),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.test.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "918749",
                    Username = "bob",
                    Password = "Password_1",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "bob@test.com"),
                        new Claim(JwtClaimTypes.WebSite, "https://bob.test.com")
                    }
                }
            };
        }
    }
}