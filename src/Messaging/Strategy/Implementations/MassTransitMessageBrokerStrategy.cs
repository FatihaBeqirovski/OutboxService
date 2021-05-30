using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using OutboxService.Exceptions;
using OutboxService.Messaging.Interfaces;
using OutboxService.Messaging.Strategy.Interfaces;
using OutboxService.Utils.Enums;
using RabbitMQ.Client;

namespace OutboxService.Messaging.Strategy.Implementations
{
    [ExcludeFromCodeCoverage]
    public class MassTransitMessageBrokerStrategy : IMessageBrokerStrategy
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private IModel _channel;
        
        public MassTransitMessageBrokerStrategy(MessagingConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(configuration.GetRabbitConnectionUri())
            };

            _connection = _connectionFactory.CreateConnection();
            InitChannel();
        }
        
        public async Task PublishAsync(IMessage message)
        {
            var massTransitMessage = MassTransitMessageBodyFactory.Create(message, _connectionFactory.Uri.Host);
            var body = Encoding.UTF8.GetBytes(massTransitMessage);

            try
            {
                if (!_channel.IsOpen)
                {
                    throw new MessageBrokerUnavailableException();
                }

                await Task.Factory.StartNew(() => Publish(message.Exchange, body));
            }
            catch (MessageBrokerUnavailableException)
            {
                throw;
            }
            catch (Exception e)
            {
                InitChannel();
                throw new MessageBrokerDeliveryFailedException($"Message delivery failed. {e.Message}", e);
            }
        }

        public BrokerType BrokerType { get; set; } = BrokerType.RabbitMq;

        private void InitChannel()
        {
            if (!_connection.IsOpen)
            {
                return;
            }

            _channel = _connection.CreateModel();
        }

        private void Publish(string exchange, byte[] body)
        {
            _channel.ExchangeDeclare(exchange, ExchangeType.Fanout, durable:true);
            _channel.BasicPublish(exchange: exchange,
                routingKey: string.Empty,
                basicProperties: null,
                body: body);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}