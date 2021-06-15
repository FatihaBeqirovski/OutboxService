using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace OutboxService.Tests.Utils
{
    public class FakeConfigurationSection : IConfigurationSection
    {
        private string _value;

        public FakeConfigurationSection(string value)
        {
            _value = value;
        }

        public string Value
        {
            get => _value;
            set => _value = value;
        }

        public IConfigurationSection GetSection(string key) => throw new System.NotImplementedException();
        public IEnumerable<IConfigurationSection> GetChildren() => throw new System.NotImplementedException();
        public IChangeToken GetReloadToken() => throw new System.NotImplementedException();

        public string this[string key]
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public string Key { get; }
        public string Path { get; }
    }
}