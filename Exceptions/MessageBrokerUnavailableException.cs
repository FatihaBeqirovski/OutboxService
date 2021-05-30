using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace OutboxService.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class MessageBrokerUnavailableException : Exception
    {
        public MessageBrokerUnavailableException()
        {
        }
        
        public MessageBrokerUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
        protected MessageBrokerUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}