using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OutboxService.Queue;

namespace OutboxService.Daemon
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseOutboxDaemon : BackgroundService
    {
        private readonly ILogger<OutboxDaemon> _logger;
        private readonly IOutboxCoordinator _coordinator;
        
        protected BaseOutboxDaemon(ILogger<OutboxDaemon> logger, IOutboxDispatcher outboxDispatcher, IPollingQueue pollingQueue)
        {
            _logger = logger;
            _coordinator = new OutboxCoordinator(outboxDispatcher, pollingQueue, _logger);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _coordinator.StartAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
        }

        protected abstract void OnStop();
        protected abstract void OnStart();
    }
}