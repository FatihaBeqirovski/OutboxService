using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using OutboxService.Database.Interfaces;

namespace OutboxService.Database.Implementations
{
    [ExcludeFromCodeCoverage]
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public IDbConnection GetConnection()
        {
            var connectionString = _configuration.GetValue<string>("ConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("ConnectionString");
            }
                
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}