using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace OutboxService.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class MessageBrokerDeliveryFailedException : Exception
    {
        public MessageBrokerDeliveryFailedException()
        {
        }
        
        public MessageBrokerDeliveryFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
        protected MessageBrokerDeliveryFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}