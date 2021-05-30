using System.Threading;
using System.Threading.Tasks;
using OutboxService.Entities;
using OutboxService.Utils;

namespace OutboxService.Queue
{
    public class PollingQueue : IPollingQueue
    {
        private readonly IPollingSource _pollingSource;
        private readonly int _waitDuration;

        public PollingQueue(IPollingSource pollingSource, int waitDuration)
        {
            _pollingSource = pollingSource;
            _waitDuration = waitDuration;
        }

        public async Task<OutboxItem[]> DequeueAsync(CancellationToken cancellationToken)
        {
            var result = await  _pollingSource.GetNextAsync(OutboxConstants.PrefetchCount, OutboxConstants.RecoveryPeriodAfterInMilliseconds);
            while (result.Length == 0 && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_waitDuration, cancellationToken);
                result = await  _pollingSource.GetNextAsync(OutboxConstants.PrefetchCount, OutboxConstants.RecoveryPeriodAfterInMilliseconds);
            }
            return result;
        }
    }
}