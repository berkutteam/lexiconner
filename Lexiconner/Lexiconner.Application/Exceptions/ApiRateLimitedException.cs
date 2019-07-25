using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    public class ApiRateLimitedException : ApplicationBaseException
    {
        private const string _title = "Api is rate limited. Try later.";

        public ApiRateLimitedException() : base(_title)
        {

        }

        public ApiRateLimitedException(string message) : base(_title, message)
        {
        }

        protected ApiRateLimitedException(string message, Exception innerException) : base(_title, message, innerException)
        {

        }
    }
}
