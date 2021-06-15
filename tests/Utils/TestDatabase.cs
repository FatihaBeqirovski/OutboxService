using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using OutboxService.Database.Interfaces;

namespace OutboxService.Tests.Utils
{
    [ExcludeFromCodeCoverage]
    public class TestDatabase : IDbConnectionFactory
    {
        public IDbConnection GetConnection()
        {
            var sqlConnection = new SQLiteConnection("Data Source=:memory:");
            sqlConnection.Open();
            sqlConnection.Execute("attach ':memory:' as [Playground]");
            SQLiteFunction.RegisterFunction(typeof(GetDateFunction));
            SQLiteFunction.RegisterFunction(typeof(SysDateTimeFunction));
            return sqlConnection;
        }

        public void Dispose()
        {
            //ignore
        }
    }
}