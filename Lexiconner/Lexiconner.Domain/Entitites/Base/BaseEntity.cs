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


        public string Id { get; set; }
    }
}
