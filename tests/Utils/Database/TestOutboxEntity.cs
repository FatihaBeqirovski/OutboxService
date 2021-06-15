using System;
using Dapper.Contrib.Extensions;
using OutboxService.Utils.Enums;

namespace OutboxService.Tests.Utils.Database
{
    [Table("Playground.Outbox")]
    public class TestOutboxEntity
    {
        [Key] public long Id { get; set; }
        public string EventId { get; set; }
        public string Body { get; set; }
        public string Exchange { get; set; }
        public byte StatusId { get; set; }
        public DateTime? PickedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public byte DeliveryCount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? BrokerTypeId { get; set; } = (long) BrokerType.RabbitMq;
    }
}