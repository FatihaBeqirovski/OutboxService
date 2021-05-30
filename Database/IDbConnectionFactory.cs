using System.Data;

namespace OutboxService.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}