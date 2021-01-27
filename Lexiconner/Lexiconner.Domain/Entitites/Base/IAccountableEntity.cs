using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.Base
{
    public interface IAccountableEntity
    {
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }
    }
}
