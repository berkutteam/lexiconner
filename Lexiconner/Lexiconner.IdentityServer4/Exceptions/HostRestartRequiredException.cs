using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4.Exceptions
{
    public class HostRestartRequiredException : Exception
    {
        public HostRestartRequiredException() : base()
        {

        }

        public HostRestartRequiredException(string message) : base(message)
        {

        }
    }
}
