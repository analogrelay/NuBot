using System;
using Moq;
using NuBot.Configuration;
using Xunit;

namespace NuBot.Test.Configuration
{
    public class KeyValueConfigurationFacts
    {
        public class TheGetSettingExtensionMethod
        {
            [Fact]
            public void ShouldReturnDefaultOfTIfStringValueIsNull()
            {
                var mockConfig = new Mock<IKeyValueConfiguration>();
                mockConfig.Setup(c => c.GetSetting("Test")).Returns<string>(null);
                Assert.Null(mockConfig.Object.GetSetting("Test", s => new Uri(s)));
            }

            [Fact]
            public void ShouldReturnProvidedDefaultIfStringValueIsNull()
            {
                var mockConfig = new Mock<IKeyValueConfiguration>();
                mockConfig.Setup(c => c.GetSetting("Test")).Returns<string>(null);
                Assert.Equal(
                    "http://microsoft.com/",
                    mockConfig.Object.GetSetting("Test", s => new Uri(s), new Uri("http://microsoft.com")).AbsoluteUri);
            }

            [Fact]
            public void ShouldReturnDefaultOfTIfStringValueIsEmpty()
            {
                var mockConfig = new Mock<IKeyValueConfiguration>();
                mockConfig.Setup(c => c.GetSetting("Test")).Returns(String.Empty);
                Assert.Null(mockConfig.Object.GetSetting("Test", s => new Uri(s)));
            }

            [Fact]
            public void ShouldReturnProvidedDefaultIfStringValueIsEmpty()
            {
                var mockConfig = new Mock<IKeyValueConfiguration>();
                mockConfig.Setup(c => c.GetSetting("Test")).Returns(String.Empty);
                Assert.Equal(
                    "http://microsoft.com/",
                    mockConfig.Object.GetSetting("Test", s => new Uri(s), new Uri("http://microsoft.com")).AbsoluteUri);
            }

            [Fact]
            public void ShouldReturnConvertedValueWhenValueIsNonNull()
            {
                var mockConfig = new Mock<IKeyValueConfiguration>();
                mockConfig.Setup(c => c.GetSetting("Test")).Returns("http://microsoft.com");
                Assert.Equal(
                    "http://microsoft.com/",
                    mockConfig.Object.GetSetting("Test", s => new Uri(s)).AbsoluteUri);
                Assert.Equal(
                    "http://microsoft.com/",
                    mockConfig.Object.GetSetting("Test", s => new Uri(s), new Uri("http://nuget.org")).AbsoluteUri);
            }
        }
    }
}