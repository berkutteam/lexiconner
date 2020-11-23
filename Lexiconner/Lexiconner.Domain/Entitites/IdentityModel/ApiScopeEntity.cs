using IdentityServer4.Models;
using Lexiconner.Domain.Entitites.Base;
using NUlid;
using NUlid.Rng;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.IdentityModel
{
    /// <summary>
    /// As I understood this is replacement for ApiResource in IdentityServer4 v4
    /// </summary>
    public class ApiScopeEntity : IIdentifiableEntity
    {
        public ApiScopeEntity()
        {
            Id = Ulid.NewUlid(new CSUlidRng()).ToString();
            ApiScope = new ApiScope();
        }
        public ApiScopeEntity(ApiScope resource)
        {
            Id = Ulid.NewUlid(new CSUlidRng()).ToString();
            ApiScope = resource;
        }

        public ApiScope ApiScope { get; set; }

        public string Id { get; set; }
    }

}
