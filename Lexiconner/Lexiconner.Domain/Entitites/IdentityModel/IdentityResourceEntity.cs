using IdentityServer4.Models;
using Lexiconner.Domain.Entitites.Base;
using MongoDB.Bson;
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
            Id = ObjectId.GenerateNewId().ToString();
            IdentityResource = new IdentityResource();
        }

        public IdentityResourceEntity(IdentityResource resource)
        {
            Id = ObjectId.GenerateNewId().ToString();
            IdentityResource = resource;
        }

        public IdentityResource IdentityResource { get; set; }

        public string Id { get; set; }
    }
}
