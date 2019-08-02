using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;


namespace Lexiconner.Infrastructure.Tests.Exceptions
{
    public class HttpStatusException : Exception
    {
        private static string GetFormattedMessage(HttpResponseMessage httpResponseMessage)
        {
            string stringResponse = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var message = $"Response status code does not indicate success: {httpResponseMessage.StatusCode}. Server response: {stringResponse}";
            return message;
        }

        public HttpResponseMessage HttpResponseMessage;
        public HttpStatusCode HttpStatusCode => HttpResponseMessage.StatusCode;

        public HttpStatusException(HttpResponseMessage httpResponseMessage) : base(GetFormattedMessage(httpResponseMessage))
        {
            this.HttpResponseMessage = httpResponseMessage;
        }
    }
}
