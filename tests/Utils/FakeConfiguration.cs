using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using OutboxService.Messaging.Strategy;

namespace OutboxService.Tests.Utils
{
    public class FakeConfiguration : IConfiguration
    {
        private static readonly IDictionary<string, string> TrueConfigValues = new Dictionary<string, string>()
        {
            {MessagingConfiguration.RabbitMqUrlKey, "rabbitmq://localhost:/"},
            {MessagingConfiguration.RabbitMqUsernameKey, "user"},
            {MessagingConfiguration.RabbitMqPassKey, "password"}
        };

        private IDictionary<string, string> _configValues = TrueConfigValues;

        public IDictionary<string, string> ConfigValues
        {
            get => _configValues;
            set => _configValues = value;
        }

        public IConfigurationSection GetSection(string key)
        {
            return new FakeConfigurationSection(_configValues[key]);
        }

        public IEnumerable<IConfigurationSection> GetChildren() => throw new System.NotImplementedException();
        public IChangeToken GetReloadToken() => throw new System.NotImplementedException();

        public string this[string key]
        {
            get => _configValues[key];
            set => throw new System.NotImplementedException();
        }
    }
}