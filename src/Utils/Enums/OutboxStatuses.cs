namespace OutboxService.Utils.Enums
{
    public enum OutboxStatuses : byte
    {
        Entered = 1,
        Inprogress = 2,
        Delivered = 3,
        Cancelled = 99
    }
}