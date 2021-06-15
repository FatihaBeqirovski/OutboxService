using System;
using Microsoft.Extensions.Configuration;
using OutboxService.Database.Interfaces;

namespace OutboxService.Database.Implementations
{
    public class RepositoryConfiguration : IRepositoryConfiguration
    {
        public static readonly string TableNameKey = "TableName";
        
        private readonly IConfiguration _configuration;
        private string _tableName;

        public RepositoryConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetTableName()
        {
            if (_tableName == null)
            {
                _tableName = _configuration.GetValue<string>(TableNameKey);
                if (string.IsNullOrWhiteSpace(_tableName))
                {
                    throw new Exception(TableNameKey);
                }

                if (_tableName.Contains("--"))
                {    
                    throw new ArgumentException("Invalid table name. Table names cannot contain double dashes");
                }
            }

            return _tableName;
        }
    }
}