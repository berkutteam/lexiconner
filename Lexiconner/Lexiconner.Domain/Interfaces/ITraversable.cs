using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Interfaces
{
    public interface ITraversable<T>
    {
        T Current { get; }
        List<T> Children { get; set; }
    }
}
