using System.Threading;
using System.Threading.Tasks;

namespace OutboxService.Coordinator
{
    public interface IOutboxCoordinator
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}