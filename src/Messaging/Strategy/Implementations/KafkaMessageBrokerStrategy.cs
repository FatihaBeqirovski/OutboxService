using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using OutboxService.Exceptions;
using OutboxService.Messaging.Interfaces;
using OutboxService.Messaging.Strategy.Interfaces;
using OutboxService.Utils.Enums;

namespace OutboxService.Messaging.Strategy.Implementations
{
    public class KafkaMessageBrokerStrategy : IMessageBrokerStrategy
    {
        private readonly IProducer<string, string> _producer;

        public KafkaMessageBrokerStrategy(MessagingConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration.GetKafkaBrokers(),
                Acks = Acks.Leader,
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync(IMessage message)
        {
            var kafkaMessage = new Message<string, string>
            {
                Timestamp = new Timestamp(DateTime.Now),
                Key = message.EventId,
                Value = message.Body
            };
            try
            {
                await _producer.ProduceAsync(message.Exchange, kafkaMessage);
            }
            catch (ProduceException<string, string> e)
            {
                throw new MessageBrokerUnavailableException(
                    $"Message produce failed. Error Message: {e.Message} --> Topic: {message.Exchange}", e);
            }
            catch (Exception e)
            {
                throw new MessageBrokerDeliveryFailedException(
                    $"Message delivery failed. Error Message: {e.Message} --> Topic: {message.Exchange}", e);
            }
        }

        public BrokerType BrokerType { get; set; } = BrokerType.Kafka;

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}