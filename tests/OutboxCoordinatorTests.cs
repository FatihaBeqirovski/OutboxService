using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OutboxService.Coordinator;
using OutboxService.Dispatcher;
using OutboxService.Entities;
using OutboxService.Queue;

namespace OutboxService.Tests
{
    public class OutboxCoordinatorTests
    {
        private Fixture _fixture;
        private Mock<IOutboxDispatcher> _outboxDispatcher;
        private Mock<IPollingQueue> _pollingQueue;
        private Mock<ILogger<OutboxCoordinator>> _logger;
        private OutboxCoordinator _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _outboxDispatcher = new Mock<IOutboxDispatcher>();
            _pollingQueue = new Mock<IPollingQueue>();
            _logger = new Mock<ILogger<OutboxCoordinator>>();
            _sut = new OutboxCoordinator(_outboxDispatcher.Object, _pollingQueue.Object, _logger.Object);
        }

        [Test]
        public async Task Start()
        {
            //Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            Func<OutboxItem[]> outboxItemFactory = () => _fixture.CreateMany<OutboxItem>(1).ToArray();

            _pollingQueue.Setup(queue => queue.DequeueAsync(cancellationToken)).ReturnsAsync(outboxItemFactory);

            //Act
            var task = Task.Run(() => _sut.StartAsync(cancellationToken));
            await Task.Delay(20);
            cancellationTokenSource.Cancel();

            //Assert
            _pollingQueue.Verify(queue => queue.DequeueAsync(cancellationToken));
            _outboxDispatcher.Verify(dispatcher => dispatcher.DispatchAsync(It.IsAny<OutboxItem>()));
            cancellationTokenSource.Cancel();
        }
    }
}