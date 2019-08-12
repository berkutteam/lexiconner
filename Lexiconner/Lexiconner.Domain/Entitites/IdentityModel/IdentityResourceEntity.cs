using IdentityServer4.Models;
using Lexiconner.Domain.Entitites.Base;
using NUlid;
using NUlid.Rng;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.IdentityModel
{
    public class IdentityResourceEntity : IIdentifiableEntity
    {
        public IdentityResourceEntity()
        {
            Id = Ulid.NewUlid(new CSUlidRng()).ToString();
            IdentityResource = new IdentityResource();
        }
        public IdentityResourceEntity(IdentityResource resource)
        {
            Id = Ulid.NewUlid(new CSUlidRng()).ToString();
            IdentityResource = resource;
        }

        public IdentityResource IdentityResource { get; set; }

        public string Id { get; set; }
    }
}
