using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.Base
{
    public interface IAccountableEntity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }
    }
}
