using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TechBlog.Domain.Extensions;

namespace TechBlog.UnitTests.Domain.Extensions
{
    public sealed class ConfigurationExtensionsTests
    {
        [Fact]
        public void GetValueOrThrow_WhenKeyIsNotImplemented_ThrowsArgumentException()
        {
            try 
            {
                // Arrange
                var configuration = new ConfigurationBuilder().Build();
                var key = "key";

                configuration.GetValue<string>(key);

                // Act
                Action act = () => configuration.GetValueOrThrow<string>(key);

                act.Should().Throw<ArgumentException>().WithMessage("Configuration don't have key implemented (Parameter 'key')");

            }
            catch (ArgumentException ex) 
            {
                // Assert
                ex.Message.Should().Be("Configuration don't have key implemented");
            }
        }

        [Fact]
        public void GetValueOrThrow_WhenKeyIsImplemented()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "key", "value" }
            }).Build();

            var key = "key";

            // Act
            var response = configuration.GetValueOrThrow<string>(key);

            // Assert
            response.Should().Be("value");

        }

        [Fact]
        public void GenerateCancellationToken_WhenConfigurationIsNotImplemented_ThrowsArgumentException()
        {
            try
            {
                // Arrange
                var configuration = new ConfigurationBuilder().Build();

                // Act
                Action act = () => configuration.GenerateCancellationToken();

                act.Should().Throw<ArgumentException>().WithMessage("Configuration don't have key implemented (Parameter 'key')");

            }
            catch (ArgumentException ex)
            {
                // Assert
                ex.Message.Should().Be("Configuration don't have key implemented");
            }
        }

        [Fact]
        public void GenerateCancellationToken_WhenConfigurationIsImplemented()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Gateways:Event:Cancellation:Hours", "1" },
                { "Gateways:Event:Cancellation:Minutes", "1" },
                { "Gateways:Event:Cancellation:Seconds", "1" }
            }).Build();

            // Act
            var response = configuration.GenerateCancellationToken();

            // Assert
            response.Should().NotBeNull();
        }
    }
}
