using System.Threading;
using System.Threading.Tasks;

namespace OutboxService
{
    public interface IOutboxCoordinator
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}