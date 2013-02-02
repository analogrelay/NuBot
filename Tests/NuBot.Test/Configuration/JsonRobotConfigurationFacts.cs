using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuBot.Configuration;
using NuBot.Test.Abstractions;
using Xunit;
using Xunit.Extensions;

namespace NuBot.Test.Configuration
{
    public class JsonRobotConfigurationFacts
    {
        public class TheConstructor
        {
            [Theory]
            [InlineData("42")]
            [InlineData("{")]
            [InlineData("{'Settings':42}")]
            [InlineData("{'Settings':[]}")]
            [InlineData("{'Settings':''}")]
            [InlineData("{'Parts':42}")]
            [InlineData("{'Parts':[]}")]
            [InlineData("{'Parts':''}")]
            [InlineData("{'Parts':{'Foo':42}}")]
            [InlineData("{'Parts':{'Foo':[]}}")]
            [InlineData("{'Parts':{'Foo':''}}")]
            public void ShouldThrowIfDataInvalid(string invalidDocument)
            {
                Assert.Throws<InvalidDataException>(() => new JsonRobotConfiguration(new TestTextFile(invalidDocument)));
            }

            [Fact]
            public void ShouldInitializeKeyValuePairsFromLoader()
            {
                const string configString = @"
{
    'Settings': {
        'Global1': 42,
        'Global2': 'Foo'
    }
}";

                // Arrange
                var configFile = new TestTextFile(configString);
                
                // Act
                var config = new JsonRobotConfiguration(configFile);

                // Assert
                Assert.Equal(42, config.GetSetting("Global1", Int32.Parse));
                Assert.Equal("Foo", config.GetSetting("Global2"));
            }

            [Fact]
            public void ShouldInitializePartConfigurations()
            {
                const string configString = @"
{
    'Parts': {
        'Part1': {
            'Enabled': true,
            'Settings': {
                'Local1': 42
            }
        },
        'Part2': {
            'Enabled': false,
            'Settings': {
                'Local2': 'Foo'
            }
        }
    }
}";

                // Arrange
                var configFile = new TestTextFile(configString);

                // Act
                var config = new JsonRobotConfiguration(configFile);

                // Assert
                var part1 = config.Parts.First();
                Assert.Equal("Part1", part1.Name);
                Assert.True(part1.IsEnabled);
                Assert.Equal(42, part1.GetSetting("Local1", Int32.Parse));

                var part2 = config.Parts.Last();
                Assert.Equal("Part2", part2.Name);
                Assert.False(part2.IsEnabled);
                Assert.Equal("Foo", part2.GetSetting("Local2"));
            }

            [Fact]
            public void ShouldAllowPartKeysToHaveBooleanValuesToEnableThemWithoutSpecifyingConfig()
            {
                const string configString = @"
{
    'Parts': {
        'Part1': true,
        'Part2': false
    }
}";

                // Arrange
                var configFile = new TestTextFile(configString);

                // Act
                var config = new JsonRobotConfiguration(configFile);

                // Assert
                var part1 = config.Parts.First();
                Assert.Equal("Part1", part1.Name);
                Assert.True(part1.IsEnabled);
                
                var part2 = config.Parts.Last();
                Assert.Equal("Part2", part2.Name);
                Assert.False(part2.IsEnabled);
            }
        }
    }
}
