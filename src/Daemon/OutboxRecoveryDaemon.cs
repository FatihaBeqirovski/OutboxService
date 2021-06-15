using Microsoft.Extensions.Logging;
using OutboxService.Dispatcher;
using OutboxService.Queue;
using OutboxService.Utils;

namespace OutboxService.Daemon
{
    public class OutboxRecoveryDaemon : BaseOutboxDaemon
    {
        private readonly ILogger<OutboxDaemon> _logger;

        public OutboxRecoveryDaemon(ILogger<OutboxDaemon> logger, IOutboxDispatcher outboxDispatcher, ISqlRecoveryPollingSource pollingSource) 
        
            : base(logger, outboxDispatcher, new PollingQueue(pollingSource, OutboxConstants.RecoveryQueueWaitDuration))
        {
            _logger = logger;
        }
        
        protected override void OnStart()
        {
            _logger.LogInformation("Starting outbox recovery daemon");
        }
        
        protected override void OnStop()
        {
            _logger.LogInformation("Stopping outbox recovery daemon");
        }
    }
}