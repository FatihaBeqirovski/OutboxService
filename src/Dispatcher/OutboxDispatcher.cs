using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OutboxService.Database.Interfaces;
using OutboxService.Entities;
using OutboxService.Exceptions;
using OutboxService.Messaging.Strategy.Interfaces;
using OutboxService.Utils;
using OutboxService.Utils.Enums;

namespace OutboxService.Dispatcher
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
            using var dbConnection = _dbConnectionFactory.GetConnection();
            try
            {
                var messageBrokerStrategy =
                    _brokerStrategies.FirstOrDefault(x => x.BrokerType == (BrokerType) outboxItem.BrokerTypeId);

                if (messageBrokerStrategy == null)
                    throw new NotImplementedException($"Strategy not found BrokerTypeId : {outboxItem.BrokerTypeId}");

                await messageBrokerStrategy.PublishAsync(outboxItem);

                await _outboxRepository.UpdateAsDeliveredAsync(dbConnection, outboxItem);
            }
            catch (MessageBrokerUnavailableException)
            {
                await HandleBrokerUnavailableException(dbConnection, outboxItem);
                throw;
            }
            catch (MessageBrokerDeliveryFailedException)
            {
                await HandleDeliveryException(dbConnection, outboxItem);
                throw;
            }
            catch (Exception)
            {
                await HandleGenericException(dbConnection, outboxItem);
                throw;
            }
        }

        private async Task HandleBrokerUnavailableException(IDbConnection dbConnection, OutboxItem outboxItem)
        {
            await _outboxRepository.ResetStatusAsync(dbConnection, outboxItem);
            await Task.Delay(OutboxConstants.RedeliveryDelay);
        }

        private async Task HandleDeliveryException(IDbConnection dbConnection, OutboxItem outboxItem)
        {
            if (outboxItem.IsDeliveryCountLessThan(OutboxConstants.RedeliveryMaxThresholdForBrokerErrors))
            {
                await _outboxRepository.ResetStatusAndIncrementCounterAsync(dbConnection, outboxItem);
                await Task.Delay(OutboxConstants.RedeliveryDelay);
            }
            else
            {
                await _outboxRepository.Cancel(dbConnection, outboxItem);
            }
        }

        private async Task HandleGenericException(IDbConnection dbConnection, OutboxItem outboxItem)
        {
            if (outboxItem.IsDeliveryCountLessThan(OutboxConstants.RedeliveryMaxThreshold))
            {
                await _outboxRepository.ResetStatusAndIncrementCounterAsync(dbConnection, outboxItem);
            }
            else
            {
                await _outboxRepository.Cancel(dbConnection, outboxItem);
            }
        }
    }
}