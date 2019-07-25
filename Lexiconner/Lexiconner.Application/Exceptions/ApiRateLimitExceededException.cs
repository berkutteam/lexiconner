using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    public class ApiRateLimitExceededException : ApplicationBaseException
    {
        private const string _title = "Api rate limit is exceeded. Try later.";

        public ApiRateLimitExceededException() : base(_title)
        {

        }

        public ApiRateLimitExceededException(string message) : base(_title, message)
        {
        }

        protected ApiRateLimitExceededException(string message, Exception innerException) : base(_title, message, innerException)
        {

        }
    }
}
