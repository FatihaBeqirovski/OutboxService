namespace OutboxService.Utils
{
    public static class OutboxConstants
    {
        public static readonly int RedeliveryMaxThreshold = 5;
        public static readonly int RedeliveryMaxThresholdForBrokerErrors = 10;
        public static readonly int RedeliveryDelay = 250;
        public static readonly int RecoveryPeriodAfterInMilliseconds = 60000;
        public static readonly int PrefetchCount = 1;
        public static readonly int QueueWaitDuration = 100;
        public static readonly int RecoveryQueueWaitDuration = 1000;
    }
}