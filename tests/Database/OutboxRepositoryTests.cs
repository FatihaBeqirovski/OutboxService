using System;
using System.Data;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using OutboxService.Database.Implementations;
using OutboxService.Entities;
using OutboxService.Tests.Utils;
using OutboxService.Tests.Utils.Database;
using OutboxService.Utils.Enums;

namespace OutboxService.Tests.Database
{
    public class OutboxRepositoryTests
    {
        private Fixture _fixture;
        private IDbConnection _dbConnection;
        private OutboxRepository _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _dbConnection = new TestDatabase().GetConnection();
            _dbConnection.CreateOutboxTable();
            _sut = new OutboxRepository(new FakeRepositoryConfiguration());
        }

        [TearDown]
        public void TearDown()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
        }

        [Test]
        public void UpdateAsDeliveredAsync_TrueStory()
        {
            //Arrange
            var outboxEntity = _fixture.Build<TestOutboxEntity>()
                .With(o => o.StatusId, (byte) OutboxStatuses.Inprogress)
                .With(o => o.PickedDate, DateTime.Now)
                .Without(o => o.DeliveryCount)
                .Without(o => o.DeliveredDate)
                .Create();
            
            DatabaseUtils.CreateOutboxItem(_dbConnection, outboxEntity);

            var outboxItem = _fixture.Build<OutboxItem>()
                .With(o => o.Id, outboxEntity.Id)
                .With(o => o.Body, outboxEntity.Body)
                .With(o => o.Exchange, outboxEntity.Exchange)
                .With(o => o.DeliveryCount, outboxEntity.DeliveryCount)
                .Create();

            //Act
            _sut.UpdateAsDeliveredAsync(_dbConnection, outboxItem).Wait();

            //Assert
            var actual  = DatabaseUtils.GetAllOutboxEntities(_dbConnection).First();
            actual.StatusId.Should().Be((byte) OutboxStatuses.Delivered);
            actual.DeliveredDate.Should().NotBeNull();
            actual.DeliveryCount.Should().Be(1);
        }
        
        [Test]
        public void ResetStatusAndIncrementCounterAsync_TrueStory()
        {
            //Arrange
            const int deliveryCount = 1;
            var outboxEntity = _fixture.Build<TestOutboxEntity>()
                .With(o => o.StatusId, (byte) OutboxStatuses.Inprogress)
                .With(o => o.PickedDate, DateTime.Now)
                .With(o => o.DeliveryCount, deliveryCount)
                .Without(o => o.DeliveredDate)
                .Create();
            
            DatabaseUtils.CreateOutboxItem(_dbConnection, outboxEntity);

            var outboxItem = _fixture.Build<OutboxItem>()
                .With(o => o.Id, outboxEntity.Id)
                .With(o => o.Body, outboxEntity.Body)
                .With(o => o.Exchange, outboxEntity.Exchange)
                .With(o => o.DeliveryCount, outboxEntity.DeliveryCount)
                .Create();

            //Act
            _sut.ResetStatusAndIncrementCounterAsync(_dbConnection, outboxItem).Wait();

            //Assert
            var actual  = DatabaseUtils.GetAllOutboxEntities(_dbConnection).First();
            actual.StatusId.Should().Be((byte) OutboxStatuses.Entered);
            actual.DeliveryCount.Should().Be(deliveryCount + 1);
        }

        [Test]
        public void ResetStatusAsync_TrueStory()
        {
            //Arrange
            const int deliveryCount = 1;
            var outboxEntity = _fixture.Build<TestOutboxEntity>()
                .With(o => o.StatusId, (byte) OutboxStatuses.Inprogress)
                .With(o => o.PickedDate, DateTime.Now)
                .With(o => o.DeliveryCount, deliveryCount)
                .Without(o => o.DeliveredDate)
                .Create();
            
            DatabaseUtils.CreateOutboxItem(_dbConnection, outboxEntity);

            var outboxItem = _fixture.Build<OutboxItem>()
                .With(o => o.Id, outboxEntity.Id)
                .With(o => o.Body, outboxEntity.Body)
                .With(o => o.Exchange, outboxEntity.Exchange)
                .With(o => o.DeliveryCount, outboxEntity.DeliveryCount)
                .Create();

            //Act
            _sut.ResetStatusAsync(_dbConnection, outboxItem).Wait();

            //Assert
            var actual  = DatabaseUtils.GetAllOutboxEntities(_dbConnection).First();
            actual.StatusId.Should().Be((byte) OutboxStatuses.Entered);
            actual.DeliveryCount.Should().Be(deliveryCount);
        }
        
        [Test]
        public void Cancel_TrueStory()
        {
            //Arrange
            const int deliveryCount = 1;
            var outboxEntity = _fixture.Build<TestOutboxEntity>()
                .With(o => o.StatusId, (byte) OutboxStatuses.Inprogress)
                .With(o => o.PickedDate, DateTime.Now)
                .With(o => o.DeliveryCount, deliveryCount)
                .Without(o => o.DeliveredDate)
                .Create();
            
            DatabaseUtils.CreateOutboxItem(_dbConnection, outboxEntity);

            var outboxItem = _fixture.Build<OutboxItem>()
                .With(o => o.Id, outboxEntity.Id)
                .With(o => o.Body, outboxEntity.Body)
                .With(o => o.Exchange, outboxEntity.Exchange)
                .With(o => o.DeliveryCount, outboxEntity.DeliveryCount)
                .Create();

            //Act
            _sut.Cancel(_dbConnection, outboxItem).Wait();

            //Assert
            var actual  = DatabaseUtils.GetAllOutboxEntities(_dbConnection).First();
            actual.StatusId.Should().Be((byte) OutboxStatuses.Cancelled);
            actual.DeliveryCount.Should().Be(deliveryCount + 1);
        }
    }
}