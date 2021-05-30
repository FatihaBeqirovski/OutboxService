using System.Threading;
using System.Threading.Tasks;
using OutboxService.Entities;

namespace OutboxService.Queue
{
    public interface IPollingQueue
    {
        Task<OutboxItem[]> DequeueAsync(CancellationToken cancellationToken);
    }
}