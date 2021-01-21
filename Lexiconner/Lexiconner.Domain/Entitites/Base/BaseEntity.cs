using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NUlid;
using NUlid.Rng;

namespace Lexiconner.Domain.Entitites.Base
{
    public class BaseEntity : IIdentifiableEntity
    {
        public BaseEntity()
        {
            RegenerateId();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public void RegenerateId()
        {
            //Id = Ulid.NewUlid(new NUlid.Rng.CSUlidRng()).ToString();
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}
