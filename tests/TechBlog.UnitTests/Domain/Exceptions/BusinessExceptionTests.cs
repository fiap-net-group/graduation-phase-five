using FluentAssertions;
using TechBlog.Domain.Exceptions;

namespace TechBlog.UnitTests.Domain.Exceptions
{
    public sealed class BusinessExceptionTests
    {
        [Fact]
        public void ShouldCreateBusinessException()
        {
            // Arrange
            var message = "This is a business exception";
            var exception = new BusinessException(message);

            // Act
            var result = exception.Message;

            // Assert
            Assert.Equal(message, result);
        }

        [Fact]
        public void ThrowIfNull_WithNonNullObject_DoesNotThrow()
        {
            // Arrange
            var obj = new object();

            // Act & Assert
            obj.Invoking(o => BusinessException.ThrowIfNull(o)).Should().NotThrow();
        }

        [Fact]
        public void ThrowIfNull_WithNullObject_ThrowsBusinessException()
        {
            // Arrange
            object obj = null;

            // Act & Assert
            Action act = () => BusinessException.ThrowIfNull(obj);
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void ShouldCreateBusinessExceptionWithErrors()
        {
            // Arrange
            var message = "This is a business exception";
            var errors = new[] { "error1", "error2" };
            var exception = new BusinessException(message, errors);

            // Act
            var result = exception.Errors;

            // Assert
            result.Should().BeEquivalentTo(errors);
        }

    }
}
