using System.Threading.Tasks;
using OutboxService.Entities;

namespace OutboxService
{
    public interface IOutboxDispatcher
    {
        Task DispatchAsync(OutboxItem outboxItem);
    }
}