using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace OutboxService.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class MissingConfigurationException: Exception
    {
        public MissingConfigurationException(string key) : base($"Missing configuration key: {key}")
        {
        }
        
        public MissingConfigurationException()
        {
        }
        
        protected MissingConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}