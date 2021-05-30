using System;
using Microsoft.Extensions.Configuration;
using OutboxService.Exceptions;

namespace OutboxService.Messaging.Strategy
{
    public class MessagingConfiguration
    {
        public static readonly string RabbitMqUrlKey = "RabbitMqUrl";
        public static readonly string RabbitMqUsernameKey = "RabbitMqUsername";
        public static readonly string RabbitMqPassKey = "RabbitMqPassword";
        private const int DefaultRabbitMqPort = 5672;
        public static readonly string KafkaUrls = "KafkaUrls";

        private readonly IConfiguration _configuration;

        public MessagingConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetRabbitConnectionUri()
        {
            var url = _configuration.GetValue<string>(RabbitMqUrlKey);
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new MissingConfigurationException(RabbitMqUrlKey);
            }

            var uri = new Uri(url);
            var port = uri.Port == -1 ? DefaultRabbitMqPort : uri.Port;
            var path = uri.PathAndQuery;

            var result = $"amqp://{GetUserName()}:{GetPassword()}@{uri.Host}:{port}{path}";
            return result;
        }

        public string GetUserName()
        {
            var result = _configuration.GetValue<string>(RabbitMqUsernameKey);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new MissingConfigurationException(RabbitMqUrlKey);
            }

            return result;
        }

        public string GetPassword()
        {
            var result = _configuration.GetValue<string>(RabbitMqPassKey);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new MissingConfigurationException(RabbitMqPassKey);
            }

            return result;
        }

        public string GetKafkaBrokers()
        {
            var result = _configuration.GetValue<string>(KafkaUrls);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new MissingConfigurationException(KafkaUrls);
            }

            return result;
        }
    }
}