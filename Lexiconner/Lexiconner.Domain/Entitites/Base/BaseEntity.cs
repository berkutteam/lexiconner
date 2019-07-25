using System;
using System.Collections.Generic;
using System.Text;
using NUlid;
using NUlid.Rng;

namespace Lexiconner.Domain.Entitites.Base
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Ulid.NewUlid(new SimpleUlidRng()).ToString();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
