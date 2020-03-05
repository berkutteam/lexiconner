using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    /// <summary>
    /// Indicates that user is unauthorized
    /// </summary>
    public class UnauthorizedException : ApplicationBaseException
    {
        private const string _title = "Unauthorized.";

        public UnauthorizedException() : base(_title)
        {

        }

        public UnauthorizedException(string message) : base(_title, message)
        {

        }
        protected UnauthorizedException(string message, Exception innerException) : base(_title, message, innerException)
        {

        }

    }
}
