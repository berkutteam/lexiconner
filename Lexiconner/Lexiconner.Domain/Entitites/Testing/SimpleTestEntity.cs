using Lexiconner.Domain.Entitites.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.Testing
{
    public class SimpleTestEntity : BaseEntity
    {
        public string Title { get; set; }
        public int Order { get; set; }
    }
}
