using OutboxService.Messaging;
using OutboxService.Utils.Enums;

namespace OutboxService.Entities
{
    public class OutboxItem : IMessage
    {
        public long Id { get; set; }
        public string Body { get; set; }
        public string Exchange { get; set; }
        public string EventId { get; set; }
        public int? DeliveryCount { get; set; }
        public long BrokerTypeId { get; set; } = (long) BrokerType.RabbitMq;

        public bool IsDeliveryCountLessThan(int than)
        {
            if (!DeliveryCount.HasValue) return true;
            return DeliveryCount < than;
        }
    }
}