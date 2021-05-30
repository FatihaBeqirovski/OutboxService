using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OutboxService.Database.Interfaces;
using OutboxService.Entities;
using OutboxService.Messaging.Strategy.Interfaces;

namespace OutboxService
{
    public class OutboxDispatcher : IOutboxDispatcher
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IEnumerable<IMessageBrokerStrategy> _brokerStrategies;

        public OutboxDispatcher(IDbConnectionFactory dbConnectionFactory, IOutboxRepository outboxRepository,
            IServiceProvider serviceProvider)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _outboxRepository = outboxRepository;
            _brokerStrategies = serviceProvider.GetServices<IMessageBrokerStrategy>();
        }

        public async Task DispatchAsync(OutboxItem outboxItem)
        {
            throw new NotImplementedException();
        }
    }
}