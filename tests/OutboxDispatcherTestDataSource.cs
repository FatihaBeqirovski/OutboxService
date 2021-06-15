using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using OutboxService.Utils.Enums;

namespace OutboxService.Tests
{
    public class OutboxDispatcherTestDataSource
    {
        public static IEnumerable BrokerTypes
        {
            get
            {
                var brokerTypes = Enum.GetValues(typeof(BrokerType))
                    .Cast<BrokerType>();

                foreach (var statusType in brokerTypes)
                {
                    yield return new TestCaseData(statusType);
                }
            }
        }
    }
}