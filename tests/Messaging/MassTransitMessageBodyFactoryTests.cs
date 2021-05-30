using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using OutboxService.Entities;
using SUT = OutboxService.Messaging.Strategy.MassTransitMessageBodyFactory;

namespace OutboxService.Tests.Messaging
{
    public class MassTransitMessageBodyFactoryTests
    {
        private Fixture _fixture;
        
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void Create_TrueStory()
        {
            //Arrange
            var outboxItem = _fixture.Build<OutboxItem>()
                .With(item => item.Body, "{ \"test1\": \"value1\", \"test2\": \"value2\"}")
                .Create();

            var host = _fixture.Create<string>();

            //Act
            var result = SUT.Create(outboxItem, host);

            //Verify
            result.Should().NotBeNull().And.NotBeEmpty();

            var document = JsonDocument.Parse(result);
            document.RootElement.GetProperty("messageId").GetString().Should().NotBeNull().And.NotBeEmpty();
            document.RootElement.GetProperty("exchangeName").GetString().Should().NotBeNull().And.NotBeEmpty().And.Be(outboxItem.Exchange);
            document.RootElement.GetProperty("destinationAddress").GetString().Should().NotBeNull().And.NotBeEmpty().And
                .Contain(outboxItem.Exchange).And.Contain(host);
            document.RootElement.GetProperty("messageType").GetArrayLength().Should().Be(1);
            document.RootElement.GetProperty("message").GetProperty("test1").GetString().Should().NotBeNull().And.NotBeEmpty().And.Be("value1");
            document.RootElement.GetProperty("message").GetProperty("test2").GetString().Should().NotBeNull().And.NotBeEmpty().And.Be("value2");
        }
    }
}