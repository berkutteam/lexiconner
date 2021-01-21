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
    /// <summary>
    /// As I understood this is replacement for ApiResource in IdentityServer4 v4
    /// </summary>
    public class ApiScopeEntity : IIdentifiableEntity
    {
        public ApiScopeEntity()
        {
            Id = ObjectId.GenerateNewId().ToString();
            ApiScope = new ApiScope();
        }

        public ApiScopeEntity(ApiScope resource)
        {
            Id = ObjectId.GenerateNewId().ToString();
            ApiScope = resource;
        }

        public ApiScope ApiScope { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }

}
