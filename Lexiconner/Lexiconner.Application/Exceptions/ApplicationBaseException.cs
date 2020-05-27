using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    public abstract class ApplicationBaseException : Exception
    {
        public string Title { get; } = "Application error.";

        protected ApplicationBaseException() : base()
        {
        }

        protected ApplicationBaseException(string title) : base()
        {
            Title = title;
        }

        protected ApplicationBaseException(string title, string message) : base(message)
        {
            Title = title;
        }

        protected ApplicationBaseException(string title, string message, Exception innerException) : base(message, innerException)
        {
            Title = title;
        }
    }
}
