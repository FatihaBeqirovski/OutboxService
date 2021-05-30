using System;
using System.Threading.Tasks;
using OutboxService.Messaging.Interfaces;
using OutboxService.Utils.Enums;

namespace OutboxService.Messaging.Strategy.Interfaces
{
    public interface IMessageBrokerStrategy : IDisposable
    {
        Task PublishAsync(IMessage message);
        public BrokerType BrokerType { get; set; }
    }
}