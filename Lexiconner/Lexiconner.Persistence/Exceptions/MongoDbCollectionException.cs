using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Lexiconner.Persistence.Exceptions
{
    /// <summary>
    /// If collection id is out of accessible scope
    /// </summary>
    public class MongoDbCollectionException : Exception
    {
        public MongoDbCollectionException()
        {
        }

        public MongoDbCollectionException(string message) : base(message)
        {
        }

        public MongoDbCollectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MongoDbCollectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
