using System.Data;
using System.Threading.Tasks;
using Dapper;
using OutboxService.Database.Interfaces;
using OutboxService.Entities;

namespace OutboxService.Database.Implementations
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly IRepositoryConfiguration _repositoryConfiguration;

        private const string UpdateAsDeliveredStatement =
            "UPDATE {0} SET StatusId = 3, DeliveredDate = SYSDATETIME(), DeliveryCount = CASE WHEN DeliveryCount IS NULL THEN 1 ELSE DeliveryCount + 1 END WHERE Id = @Id";

        private const string ResetStatusAndIncrementCounterStatement =
            "UPDATE {0} SET StatusId = 1, PickedDate = NULL, DeliveryCount = CASE WHEN DeliveryCount IS NULL THEN 1 ELSE DeliveryCount + 1 END WHERE Id = @Id";

        private const string ResetStatusStatement =
            "UPDATE {0} SET StatusId = 1, PickedDate = NULL WHERE Id = @Id";

        private const string CancelStatement =
            "UPDATE {0} SET StatusId = 99, PickedDate = NULL, DeliveryCount = CASE WHEN DeliveryCount IS NULL THEN 1 ELSE DeliveryCount + 1 END WHERE Id = @Id";

        public OutboxRepository(IRepositoryConfiguration repositoryConfiguration)
        {
            _repositoryConfiguration = repositoryConfiguration;
        }

        private string PreprocessStatement(string statement)
        {
            return string.Format(statement, _repositoryConfiguration.GetTableName());
        }
        
        public async Task UpdateAsDeliveredAsync(IDbConnection dbConnection, OutboxItem outboxItem)
        {
            await dbConnection.ExecuteAsync(PreprocessStatement(UpdateAsDeliveredStatement), new {Id = outboxItem.Id});
        }
        
        public async Task ResetStatusAndIncrementCounterAsync(IDbConnection dbConnection, OutboxItem outboxItem)
        {
            await dbConnection.ExecuteAsync(PreprocessStatement(ResetStatusAndIncrementCounterStatement), new {Id = outboxItem.Id});
        }
        
        public async Task ResetStatusAsync(IDbConnection dbConnection, OutboxItem outboxItem)
        {
            await dbConnection.ExecuteAsync(PreprocessStatement(ResetStatusStatement), new {Id = outboxItem.Id});
        }
        
        public async Task Cancel(IDbConnection dbConnection, OutboxItem outboxItem)
        {
            await dbConnection.ExecuteAsync(PreprocessStatement(CancelStatement), new {Id = outboxItem.Id});
        }
    }
}