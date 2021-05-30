using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutboxService.Entities;
using OutboxService.Queue;

namespace OutboxService.Tests.Queue
{
    public class PollingQueueTests
    {
        private Fixture _fixture;
        private Mock<IPollingSource> _pollingSource;
        private PollingQueue _sut;
        private int _waitDuration = 20;

        [SetUp]
        public void Setup()
        {
            _pollingSource = new Mock<IPollingSource>();
            _fixture = new Fixture();
            _sut = new PollingQueue(_pollingSource.Object, _waitDuration);
        }

        [Test]
        public async Task Dequeue_WhenThereArePendingItems_ShouldReturnFirst()
        {
            //Arrange
            var cancellationToken = new CancellationToken();
            
            var queuedItem = _fixture.Create<OutboxItem>();
            _pollingSource.Setup(x => x.GetNextAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new[]{ queuedItem });

            //Act
            var item = await _sut.DequeueAsync(cancellationToken);

            //Assert
            item.Should().NotBeNull();
            item.First().Should().Be(queuedItem);
        }

        [Test]
        public void Dequeue_WhenThereAreNoPendingItems_ShouldBlock()
        {
            //Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            
            //Act
            var task = _sut.DequeueAsync(cancellationToken);
            Thread.Sleep(_waitDuration * 2);
            
            //Assert
            task.IsCompleted.Should().BeFalse();
            cancellationTokenSource.Cancel();
        }
    }
}