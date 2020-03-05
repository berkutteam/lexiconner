using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    /// <summary>
    /// Indicates that operation can't be performed due to invalid request or something else
    /// </summary>
    public class BadRequestException : ApplicationBaseException
    {
        private const string _title = "Bad request.";

        public BadRequestException() : base(_title)
        {

        }

        public BadRequestException(string message) : base(_title, message)
        {

        }
        protected BadRequestException(string message, Exception innerException) : base(_title, message, innerException)
        {

        }
    }
}
