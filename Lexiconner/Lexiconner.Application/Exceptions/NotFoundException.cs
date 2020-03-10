using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    /// <summary>
    /// Indicates that resource wasn't found
    /// </summary>
    public class NotFoundException : ApplicationBaseException
    {
        private const string _title = "Not found.";

        public NotFoundException() : base(_title)
        {

        }

        public NotFoundException(string message) : base(_title, message)
        {

        }

        public NotFoundException(string message, Exception innerException) : base(_title, message, innerException)
        {

        }
    }
}
