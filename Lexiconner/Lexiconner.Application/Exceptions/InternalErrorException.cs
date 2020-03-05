using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    /// <summary>
    /// Indicates that some error occured
    /// </summary>
    public class InternalErrorException : ApplicationBaseException
    {
        private const string _title = "Internal error.";

        public InternalErrorException() : base(_title)
        {

        }

        public InternalErrorException(string message) : base(_title, message)
        {

        }

        protected InternalErrorException(string message, Exception innerException) : base(_title, message, innerException)
        {

        }
    }
}
