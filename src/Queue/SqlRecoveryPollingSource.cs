using System.Linq;
using System.Threading.Tasks;
using Dapper;
using OutboxService.Database.Interfaces;
using OutboxService.Entities;

namespace OutboxService.Queue
{
    public class SqlRecoveryPollingSource : ISqlRecoveryPollingSource
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IRepositoryConfiguration _repositoryConfiguration;

        private const string GetNextQuery =
            @"
        DECLARE @Ids TABLE (Id bigint)
                
        UPDATE TOP({0}) {1} SET
            StatusId = 2, 
            PickedDate = SYSDATETIME()
        OUTPUT Inserted.Id INTO @Ids
        WHERE StatusId = 2 AND PickedDate < DATEADD(MS, -{2}, SYSDATETIME())

        SELECT 
            Id,
            EventId,
            BrokerTypeId, 
            Body, 
            Exchange,
            DeliveryCount
        FROM {1} 
        WHERE Id IN (SELECT Id FROM @Ids)";

        public SqlRecoveryPollingSource(IDbConnectionFactory dbConnectionFactory,
            IRepositoryConfiguration repositoryConfiguration)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _repositoryConfiguration = repositoryConfiguration;
        }

        public async Task<OutboxItem[]> GetNextAsync(int batchCount, int recoverAfterMilliseconds)
        {
            using var connection = _dbConnectionFactory.GetConnection();
            var query = string.Format(GetNextQuery, batchCount, _repositoryConfiguration.GetTableName(),
                recoverAfterMilliseconds);
            var result = await connection.QueryAsync<OutboxItem>(query);
            return result.ToArray();
        }
    }
}