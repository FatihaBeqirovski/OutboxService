using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using OutboxService.Database.Interfaces;
using OutboxService.Dispatcher;
using OutboxService.Entities;
using OutboxService.Exceptions;
using OutboxService.Messaging.Strategy.Interfaces;
using OutboxService.Utils;
using OutboxService.Utils.Enums;

namespace OutboxService.Tests
{
    public class OutboxDispatcherTests
    {
        private Fixture _fixture;
        private Mock<IOutboxRepository> _outboxRepository;
        private Mock<IDbConnectionFactory> _dbConnectionFactory;
        private Mock<IDbConnection> _dbConnection;
        private OutboxDispatcher _sut;
        private Mock<IMessageBrokerStrategy>[] _brokerStrategies;
        
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            var kafkaMessageBroker = new Mock<IMessageBrokerStrategy>();
            var rabbitMessageBroker = new Mock<IMessageBrokerStrategy>();
            _outboxRepository = new Mock<IOutboxRepository>();
            _dbConnectionFactory = new Mock<IDbConnectionFactory>();
            _dbConnection = new Mock<IDbConnection>();
            _brokerStrategies = new[] {kafkaMessageBroker, rabbitMessageBroker};

            kafkaMessageBroker.Setup(x => x.BrokerType).Returns(BrokerType.Kafka);
            rabbitMessageBroker.Setup(x => x.BrokerType).Returns(BrokerType.RabbitMq);
            _dbConnectionFactory.Setup(dbConnectionFactory => dbConnectionFactory.GetConnection())
                .Returns(_dbConnection.Object);
 
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns(_brokerStrategies.Select(x => x.Object).ToList);

            _sut = new OutboxDispatcher(_dbConnectionFactory.Object, _outboxRepository.Object,
                serviceProvider.Object);
        }

        [Test, TestCaseSource(typeof(OutboxDispatcherTestDataSource),
             nameof(OutboxDispatcherTestDataSource.BrokerTypes))]
        public async Task Dispatch_TrueStory(BrokerType brokerType)
        {
            //Arrange
            var outboxItem = _fixture.Build<OutboxItem>()
                .With(o => o.BrokerTypeId, (long) brokerType)
                .Create();

            var messageBroker = _brokerStrategies.FirstOrDefault(x => x.Object.BrokerType == brokerType);

            //Act
            await _sut.DispatchAsync(outboxItem);

            //Assert
            messageBroker?.Verify(broker => broker.PublishAsync(outboxItem), Times.Once);
            _outboxRepository.Verify(repository => repository.UpdateAsDeliveredAsync(_dbConnection.Object, outboxItem),
                Times.Once);
        }

        [Test, TestCaseSource(typeof(OutboxDispatcherTestDataSource),
             nameof(OutboxDispatcherTestDataSource.BrokerTypes))]
        public void Dispatch_WhenDeliveryFailed_ShouldUpdatedToEnteredStatusAndIncrementDeliveryCount(
            BrokerType brokerType)
        {
            //Arrange
            var outboxItem = _fixture.Build<OutboxItem>()
                .With(o => o.BrokerTypeId, (long) brokerType)
                .With(x => x.DeliveryCount, OutboxConstants.RedeliveryMaxThreshold - 1)
                .Create();

            var messageBroker = _brokerStrategies.FirstOrDefault(x => x.Object.BrokerType == brokerType);

            messageBroker?.Setup(broker => broker.PublishAsync(outboxItem))
                .Throws<MessageBrokerDeliveryFailedException>();

            //Act
            Assert.ThrowsAsync<MessageBrokerDeliveryFailedException>(() => _sut.DispatchAsync(outboxItem));

            //Assert
            messageBroker?.Verify(broker => broker.PublishAsync(outboxItem), Times.Once);
            _outboxRepository.Verify(
                repository => repository.ResetStatusAndIncrementCounterAsync(_dbConnection.Object, outboxItem),
                Times.Once);
        }

        [Test, TestCaseSource(typeof(OutboxDispatcherTestDataSource),
             nameof(OutboxDispatcherTestDataSource.BrokerTypes))]
        public void Dispatch_WhenDeliveryFailedForMaximumTimes_ShouldCancel(BrokerType brokerType)
        {
            //Arrange
            var outboxItem = _fixture.Build<OutboxItem>()
                .With(o => o.BrokerTypeId, (long) brokerType)
                .With(x => x.DeliveryCount, OutboxConstants.RedeliveryMaxThresholdForBrokerErrors)
                .Create();

            var messageBroker = _brokerStrategies.FirstOrDefault(x => x.Object.BrokerType == brokerType);

            messageBroker?.Setup(broker => broker.PublishAsync(outboxItem))
                .Throws<MessageBrokerDeliveryFailedException>();

            //Act
            Assert.ThrowsAsync<MessageBrokerDeliveryFailedException>(() => _sut.DispatchAsync(outboxItem));

            //Assert
            messageBroker?.Verify(broker => broker.PublishAsync(outboxItem), Times.Once);
            _outboxRepository.Verify(repository => repository.Cancel(_dbConnection.Object, outboxItem), Times.Once);
        }

        [Test, TestCaseSource(typeof(OutboxDispatcherTestDataSource),
             nameof(OutboxDispatcherTestDataSource.BrokerTypes))]
        public void Dispatch_WhenThereIsAGenericErrorAndDeliveryCountIsExceeded_ShouldCancelItem(BrokerType brokerType)
        {
            //Arrange
            var outboxItem = _fixture.Build<OutboxItem>()
                .With(o => o.BrokerTypeId, (long) brokerType)
                .With(x => x.DeliveryCount, OutboxConstants.RedeliveryMaxThreshold)
                .Create();

            var messageBroker = _brokerStrategies.FirstOrDefault(x => x.Object.BrokerType == brokerType);

            messageBroker?.Setup(broker => broker.PublishAsync(outboxItem))
                .Throws<Exception>();

            //Act
            Assert.ThrowsAsync<Exception>(() => _sut.DispatchAsync(outboxItem));

            //Assert
            messageBroker?.Verify(broker => broker.PublishAsync(outboxItem), Times.Once);
            _outboxRepository.Verify(repository => repository.Cancel(_dbConnection.Object, outboxItem), Times.Once);
        }

        [Test, TestCaseSource(typeof(OutboxDispatcherTestDataSource),
             nameof(OutboxDispatcherTestDataSource.BrokerTypes))]
        public void Dispatch_WhenThereIsAGenericErrorAndDeliveryCountIsnotExceeded_ShouldResetItem(
            BrokerType brokerType)
        {
            //Arrange
            var outboxItem = _fixture.Build<OutboxItem>()
                .With(x => x.DeliveryCount, OutboxConstants.RedeliveryMaxThreshold - 1)
                .With(o => o.BrokerTypeId, (long) brokerType)
                .Create();
            var messageBroker = _brokerStrategies.FirstOrDefault(x => x.Object.BrokerType == brokerType);

            messageBroker?.Setup(broker => broker.PublishAsync(outboxItem))
                .Throws<Exception>();

            //Act
            Assert.ThrowsAsync<Exception>(() => _sut.DispatchAsync(outboxItem));

            //Assert
            messageBroker?.Verify(broker => broker.PublishAsync(outboxItem), Times.Once);
            _outboxRepository.Verify(
                repository => repository.ResetStatusAndIncrementCounterAsync(_dbConnection.Object, outboxItem),
                Times.Once);
        }

        [Test, TestCaseSource(typeof(OutboxDispatcherTestDataSource),
             nameof(OutboxDispatcherTestDataSource.BrokerTypes))]
        public void Dispatch_WhenBrokenIsUnavailable_ShouldReturnToQueue(BrokerType brokerType)
        {
            //Arrange
            var outboxItem = _fixture.Build<OutboxItem>()
                .With(x => x.DeliveryCount, OutboxConstants.RedeliveryMaxThreshold - 1)
                .With(o => o.BrokerTypeId, (long) brokerType)
                .Create();

            var messageBroker = _brokerStrategies.FirstOrDefault(x => x.Object.BrokerType == brokerType);
            messageBroker?.Setup(broker => broker.PublishAsync(outboxItem))
                .Throws<MessageBrokerUnavailableException>();

            //Act
            Assert.ThrowsAsync<MessageBrokerUnavailableException>(() => _sut.DispatchAsync(outboxItem));

            //Assert
            messageBroker?.Verify(broker => broker.PublishAsync(outboxItem), Times.Once);
            _outboxRepository.Verify(repository => repository.ResetStatusAsync(_dbConnection.Object, outboxItem),
                Times.Once);
        }
    }
}