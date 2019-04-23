using System;
using System.Collections.Generic;
using System.Text;
using NUlid;

namespace Lexiconner.Domain.Entitites.Base
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Ulid.NewUlid().ToString();
        }

        // [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public string Id { get; set; }
    }
}
