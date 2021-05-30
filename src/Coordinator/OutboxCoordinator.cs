using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OutboxService.Dispatcher;
using OutboxService.Queue;

namespace OutboxService.Coordinator
{
    public class OutboxCoordinator : IOutboxCoordinator
    {
        private readonly IOutboxDispatcher _outboxDispatcher;
        private readonly IPollingQueue _pollingQueue;
        private readonly ILogger _logger;

        public OutboxCoordinator(IOutboxDispatcher outboxDispatcher, IPollingQueue pollingQueue, ILogger logger)
        {
            _outboxDispatcher = outboxDispatcher;
            _pollingQueue = pollingQueue;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var items = await _pollingQueue.DequeueAsync(cancellationToken);
                foreach (var item in items)
                {
                    try
                    {
                        await _outboxDispatcher.DispatchAsync(item);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                    }
                }
            }
        }
    }
}