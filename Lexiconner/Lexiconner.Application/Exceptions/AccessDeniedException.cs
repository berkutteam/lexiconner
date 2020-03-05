using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    /// <summary>
    /// Indicates that user doesn't have enought permissions to perform action
    /// </summary>
    public class AccessDeniedException : ApplicationBaseException
    {
        private const string _title = "Access denied.";

        public AccessDeniedException() : base(_title)
        {

        }

        public AccessDeniedException(string message) : base(_title, message)
        {
        }     

        protected AccessDeniedException(string message, Exception innerException) : base(_title, message, innerException)
        {

        }
    }
}
