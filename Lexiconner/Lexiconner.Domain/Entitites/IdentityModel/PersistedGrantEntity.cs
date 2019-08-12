using IdentityServer4.Models;
using Lexiconner.Domain.Entitites.Base;
using NUlid;
using NUlid.Rng;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.IdentityModel
{
    public class PersistedGrantEntity : IIdentifiableEntity
    {

        public PersistedGrantEntity()
        {
            Id = Ulid.NewUlid(new CSUlidRng()).ToString();
            PersistedGrant = new PersistedGrant();
        }

        public PersistedGrantEntity(PersistedGrant persistedGrant)
        {
            Id = Ulid.NewUlid(new CSUlidRng()).ToString();
            PersistedGrant = persistedGrant;
        }

        public PersistedGrant PersistedGrant { get; set; }

        public string Id { get; set; }
    }
}
