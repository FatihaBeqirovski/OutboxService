using System.Data;
using System.Threading.Tasks;
using OutboxService.Entities;

namespace OutboxService.Database.Interfaces
{
    public interface IOutboxRepository
    {
        Task UpdateAsDeliveredAsync(IDbConnection dbConnection, OutboxItem outboxItem);
        Task ResetStatusAndIncrementCounterAsync(IDbConnection dbConnection, OutboxItem outboxItem);
        Task ResetStatusAsync(IDbConnection dbConnection, OutboxItem outboxItem);
        Task Cancel(IDbConnection dbConnection, OutboxItem outboxItem);
    }
}