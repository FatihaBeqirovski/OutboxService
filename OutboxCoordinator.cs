using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OutboxService.Queue;

namespace OutboxService
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}