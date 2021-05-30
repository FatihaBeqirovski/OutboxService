using OutboxService.Database.Interfaces;

namespace OutboxService.Tests.Database
{
    public class FakeRepositoryConfiguration : IRepositoryConfiguration
    {
        private const string TableName = "Playground.Outbox";
            
        public string GetTableName()
        {
            return TableName;
        }
    }
}