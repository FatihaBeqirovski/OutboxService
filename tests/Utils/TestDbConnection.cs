using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace OutboxService.Tests.Utils
{
    [ExcludeFromCodeCoverage]
    public class TestDbConnection : IDbConnection
    {
        private readonly IDbConnection _wrappedConnection;

        public TestDbConnection(IDbConnection connection)
        {
            _wrappedConnection = connection;
        }

        public void Dispose()
        {
            //Do nothing
        }

        public void DisposeBrutally()
        {
            _wrappedConnection.Dispose();
        }

        public IDbTransaction BeginTransaction()
        {
            return _wrappedConnection.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _wrappedConnection.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            _wrappedConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            _wrappedConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return _wrappedConnection.CreateCommand();
        }

        public void Open()
        {
            _wrappedConnection.Open();
        }

        public string ConnectionString
        {
            get => _wrappedConnection.ConnectionString;
            set => _wrappedConnection.ConnectionString = value;
        }

        public int ConnectionTimeout => _wrappedConnection.ConnectionTimeout;

        public string Database => _wrappedConnection.Database;

        public ConnectionState State => _wrappedConnection.State;
    }
}