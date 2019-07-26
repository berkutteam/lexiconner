using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.Base
{
    public interface IIdentifiableEntity
    {
        string Id { get; set; }
    }
}
