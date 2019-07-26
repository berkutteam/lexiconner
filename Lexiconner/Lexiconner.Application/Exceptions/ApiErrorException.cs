using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    public class ApiErrorException : ApplicationBaseException
    {
        private const string _title = "API return unsuccessfult status code.";

        public ApiErrorException() : base(_title)
        {

        }

        public ApiErrorException(string message) : base(_title, message)
        {
        }

        protected ApiErrorException(string message, Exception innerException) : base(_title, message, innerException)
        {

        }
    }
}
