using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Mapster;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.UnitTests.Domain.Extensions
{
    public sealed class ValidatorExtensionsTests
    {
        [Fact]
        public async void ThrowIfInvalidAsync_WhenInputIsValid()
        {
            // Arrange
            var validator = Substitute.For<IValidator<string>>();
            var input = "input";
            var logger = Substitute.For<ILoggerManager>();
            var cancellationToken = new CancellationToken();

            validator.ValidateAsync(input, cancellationToken).Returns(new ValidationResult());

            // Act
            await validator.ThrowIfInvalidAsync(input, logger, cancellationToken);

            // Assert
            await validator.Received(1).ValidateAsync(input, cancellationToken);
        }

        [Fact]
        public async void ThrowIfInvalidAsync_WhenInputIsInvalid()
        {
            // Arrange
            var validator = Substitute.For<IValidator<string>>();
            var input = "input";
            var logger = Substitute.For<ILoggerManager>();
            var cancellationToken = new CancellationToken();

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("property", "error message")
            });

            validator.ValidateAsync(input, cancellationToken).Returns(validationResult);

            // Act
            var exception = await Assert.ThrowsAsync<BusinessException>(() => validator.ThrowIfInvalidAsync(input, logger, cancellationToken));

            // Assert
            exception.Message.Should().Be(ResponseMessage.ValidationError.ToString());
            exception.Errors.Should().BeEquivalentTo(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        [Fact]
        public async void ValidateInputAsync_WhenInputIsValid()
        {
            // Arrange
            var validator = Substitute.For<IValidator<string>>();
            var input = "input";
            var logger = Substitute.For<ILoggerManager>();
            var cancellationToken = new CancellationToken();

            validator.ValidateAsync(input, cancellationToken).Returns(new ValidationResult());

            // Act
            var (isValid, output) = await validator.ValidateInputAsync<string, string>(input, logger, cancellationToken);

            // Assert
            isValid.Should().BeTrue();
            output.Should().BeNull();
        }

        [Fact]
        public async void ValidateInputAsync_WhenInputIsInvalid()
        {
            // Arrange
            var validator = Substitute.For<IValidator<string>>();
            var input = "input";
            var logger = Substitute.For<ILoggerManager>();
            var cancellationToken = new CancellationToken();

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("property", "error message")
            });

            validator.ValidateAsync(input, cancellationToken).Returns(validationResult);

            // Act
            var (isValid, output) = await validator.ValidateInputAsync<string, string>(input, logger, cancellationToken);

            // Assert
            isValid.Should().BeFalse();
            output.Should().BeEquivalentTo(validationResult.Adapt<string>());
        }
    }
}
