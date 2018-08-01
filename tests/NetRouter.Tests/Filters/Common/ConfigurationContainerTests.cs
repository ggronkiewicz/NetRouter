namespace NetRouter.Tests.Filters.Common
{
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NetRouter.Filters.Common;
    using System.Collections.Generic;
    using Xunit;

    public class ConfigurationContainerTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("111")]
        [InlineData("111:222")]
        [InlineData("111:22:3")]
        public void CallBackTest(string section)
        {
            string testString = "test";
            int testInt = 75;
            var configurator = new ConfigurationContainer(this.GetConfiguration(testInt, testString, section));

            ConfigurationItem result = null;
            configurator.Configure<ConfigurationItem>(section, item => { result = item; });

            result.Should().NotBeNull();
            result.TestInt.Should().Be(testInt);
            result.TestString.Should().Be(testString);
        }

        private IConfiguration GetConfiguration(int testInt, string testString, string sectionName = "config")
        {
            var dictionary = new Dictionary<string, string>
            {
                { sectionName + ":" + nameof(ConfigurationItem.TestString), testString },
                { sectionName + ":" + nameof(ConfigurationItem.TestInt), testInt.ToString() }
            };

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dictionary);

            return configurationBuilder.Build();
        }

        public class ConfigurationItem
        {
            public string TestString { get; set; }
            public int TestInt { get; set; }
        }
    }
}
