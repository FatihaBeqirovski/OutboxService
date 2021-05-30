namespace OutboxService.Messaging
{
    public interface IMessage
    {
        string Body { get; set; }
        string Exchange { get; set; }
        string EventId { get; set; }
    }
}