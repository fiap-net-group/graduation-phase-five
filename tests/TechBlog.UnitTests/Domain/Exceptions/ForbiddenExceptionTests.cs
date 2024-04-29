using FluentAssertions;
using TechBlog.Domain.Exceptions;

namespace TechBlog.UnitTests.Domain.Exceptions
{
    public sealed class ForbiddenExceptionTests
    {
        [Fact]
        public void ShouldCreateForbiddenException()
        {
            // Arrange
            var message = "This is a forbidden exception";
            var exception = new ForbiddenException(message);

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
            obj.Invoking(o => ForbiddenException.ThrowIfNull(o)).Should().NotThrow();
        }

        [Fact]
        public void ThrowIfNull_WithNullObject_ThrowsForbiddenException()
        {
            // Arrange
            object obj = null;

            // Act & Assert
            Action act = () => ForbiddenException.ThrowIfNull(obj);
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void ShouldCreateForbiddenExceptionWithErrors()
        {
            // Arrange
            var message = "This is a forbidden exception";
            var errors = new string[] { "error1", "error2" };
            var exception = new ForbiddenException(message, errors);

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
