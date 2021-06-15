using System.Threading.Tasks;
using OutboxService.Entities;

namespace OutboxService.Dispatcher
{
    public interface IOutboxDispatcher
    {
        Task DispatchAsync(OutboxItem outboxItem);
    }
}