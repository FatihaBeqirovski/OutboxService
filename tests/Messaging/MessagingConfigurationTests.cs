using FluentAssertions;
using NUnit.Framework;
using OutboxService.Messaging.Strategy;
using OutboxService.Tests.Utils;

namespace OutboxService.Tests.Messaging
{
    public class MessagingConfigurationTests
    {
        private FakeConfiguration _fakeConfiguration;
        private MessagingConfiguration _messagingConfiguration;

        [SetUp]
        public void Setup()
        {
            _fakeConfiguration = new FakeConfiguration();
            _messagingConfiguration = new MessagingConfiguration(_fakeConfiguration);
        }

        [Test]
        public void Ctor_TrueStory()
        {
            //Arrange
            var configuration = new FakeConfiguration();

            //Act
            var connectionUri = _messagingConfiguration.GetRabbitConnectionUri();
            var userName = _messagingConfiguration.GetUserName();
            var password = _messagingConfiguration.GetPassword();

            //Assert
            connectionUri.Should().Be("amqp://user:password@localhost:5672/");
        }
    }
}