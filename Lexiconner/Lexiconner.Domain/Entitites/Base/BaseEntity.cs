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
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        // [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public void RegenerateId()
        {
            //Id = Ulid.NewUlid(new NUlid.Rng.CSUlidRng()).ToString();
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}
