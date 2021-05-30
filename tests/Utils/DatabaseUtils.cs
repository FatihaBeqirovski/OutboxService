using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using OutboxService.Tests.Utils.Database;

namespace OutboxService.Tests.Utils
{
    public static class DatabaseUtils
    {
        public static void CreateOutboxTable(this IDbConnection connection)
        {
            connection.Execute(@"create table [Playground].Outbox
                                    (
                                        Id integer primary key AUTOINCREMENT,
	                                    EventId ntext NULL,
	                                    Body ntext NOT NULL,
	                                    Exchange nvarchar(100) NOT NULL,
	                                    StatusId integer  NOT NULL,
	                                    PickedDate date NULL,
	                                    DeliveredDate date NULL,
	                                    DeliveryCount integer,
	                                    CreatedBy varchar(100) NOT NULL,
	                                    CreatedDate date NOT NULL,
    									BrokerTypeId integer null
                                     )");
        }

        public static void CreateOutboxItem(IDbConnection connection, TestOutboxEntity outboxEntity)
        {
	        connection.Insert(outboxEntity);
        }

        public static IEnumerable<TestOutboxEntity> GetAllOutboxEntities(IDbConnection connection)
        {
	        return connection.GetAll<TestOutboxEntity>();
        }
    }
}