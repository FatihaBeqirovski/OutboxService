using Microsoft.Extensions.Logging;
using OutboxService.Queue;
using OutboxService.Utils;

namespace OutboxService.Daemon
{
    public class OutboxDaemon : BaseOutboxDaemon
    {
        private readonly ILogger<OutboxDaemon> _logger;

        public OutboxDaemon(ILogger<OutboxDaemon> logger, IOutboxDispatcher outboxDispatcher, ISqlPollingSource pollingSource) 
            : base(logger, outboxDispatcher, new PollingQueue(pollingSource, OutboxConstants.QueueWaitDuration))
        {
            _logger = logger;
        }
        
        protected override void OnStart()
        {
            _logger.LogInformation("Starting outbox daemon");
        }
        
        protected override void OnStop()
        {
            _logger.LogInformation("Stopping outbox daemon");
        }
    }
}