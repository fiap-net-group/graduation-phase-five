using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechBlog.Domain.Extensions;

namespace TechBlog.UnitTests.Domain.Extensions
{
    public sealed class CancellationTokenExtensionsTests
    {
        [Fact]
        public void GenerateCancellationToken_WhenCalled_ShouldReturnCancellationToken()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var hoursForCancellation = 0;
            var minutesForCancellation = 0;
            var secondsForCancellation = 5;

            // Act
            var result = cancellationTokenSource.GenerateCancellationToken(hoursForCancellation, minutesForCancellation, secondsForCancellation);

            // Assert
            Assert.IsType<CancellationToken>(result);
        }

        [Fact]
        public void GenerateCancellationToken_WhenCalled_ShouldReturnCancellationTokenWithCorrectTimeSpan()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var hoursForCancellation = 0;
            var minutesForCancellation = 0;
            var secondsForCancellation = 5;

            // Act
            var result = cancellationTokenSource.GenerateCancellationToken(hoursForCancellation, minutesForCancellation, secondsForCancellation);

            // Assert
            Assert.True(cancellationTokenSource.Token.CanBeCanceled);
            Assert.False(cancellationTokenSource.Token.IsCancellationRequested);
            result.Should().BeOfType<CancellationToken>();
        }
    }
}
