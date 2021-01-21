using IdentityServer4.Models;
using Lexiconner.Domain.Entitites.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
            Id = ObjectId.GenerateNewId().ToString();
            PersistedGrant = new PersistedGrant();
        }

        public PersistedGrantEntity(PersistedGrant persistedGrant)
        {
            Id = ObjectId.GenerateNewId().ToString();
            PersistedGrant = persistedGrant;
        }

        public PersistedGrant PersistedGrant { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
