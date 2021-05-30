using System.Threading.Tasks;
using OutboxService.Entities;

namespace OutboxService.Queue
{
    public interface IPollingSource
    {
        Task<OutboxItem[]> GetNextAsync(int batchCount, int recoverAfterMilliseconds);
    }
}