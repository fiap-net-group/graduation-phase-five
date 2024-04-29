using FluentAssertions;
using TechBlog.Domain.Exceptions;

namespace TechBlog.UnitTests.Domain.Exceptions
{
    public sealed class NotFoundExceptionTests
    {
        [Fact]
        public void ShouldCreateNotFoundException()
        {
            // Arrange
            var message = "This is a not found exception";
            var exception = new NotFoundException(message);

            // Act
            var result = exception.Errors.FirstOrDefault();

            // Assert
            Assert.Equal(message, result);
        }

        [Fact]
        public void ThrowIfNull_WithNonNullObject_DoesNotThrow()
        {
            // Arrange
            var obj = new object();

            // Act & Assert
            obj.Invoking(o => NotFoundException.ThrowIfNull(o)).Should().NotThrow();
        }

        [Fact]
        public void ThrowIfNull_WithNullObject_ThrowsNotFoundException()
        {
            // Arrange
            object obj = null;

            // Act & Assert
            Action act = () => NotFoundException.ThrowIfNull(obj);
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void ShouldCreateNotFoundExceptionWithErrors()
        {
            // Arrange
            var message = "This is a not found exception";
            var errors = new string[] { "error1", "error2" };
            var exception = new NotFoundException(message, errors);

            // Act
            var result = exception.Errors;

            // Assert
            Assert.Equal(errors.Length + 1, result.Length);
            Assert.Equal(message, result[0]);
            Assert.Equal(errors[0], result[1]);
            Assert.Null(result[2]);
        }
    }
}
