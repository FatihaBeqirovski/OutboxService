using System.Threading.Tasks;
using OutboxService.Messaging.Interfaces;
using OutboxService.Messaging.Strategy.Interfaces;

namespace OutboxService.Messaging.Strategy
{
    public class MessageBrokerService
    {
        private readonly IMessageBrokerStrategy _messageBrokerStrategy;

        public MessageBrokerService(IMessageBrokerStrategy messageBrokerStrategy)
        {
            _messageBrokerStrategy = messageBrokerStrategy;
        }

        public async Task PublishAsync(IMessage message)
        {
            await _messageBrokerStrategy.PublishAsync(message);
        }
    }
}