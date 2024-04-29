using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TechBlog.Application.User.Update;
using TechBlog.Application.User.Update.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.UnitTests.Fixtures;

namespace TechBlog.UnitTests.Application.User
{
    public sealed class UpdateUserTests
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<UpdateUserInput> _validator;
        private readonly IIdentityManager _identityManager;

        public UpdateUserTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<UpdateUserInput>>();
            _identityManager = Substitute.For<IIdentityManager>();
            MapperFixture.AddMapper(typeof(UpdateUserMapper).Assembly);
        }

        [Theory]
        [InlineData("", false, "InvalidId")]
        [InlineData(null, false, "InvalidId")]
        [InlineData("  ", false, "InvalidId")]
        [InlineData("123123", true)]
        public void Validator_ValidateId_ShouldRespectValidations(string id, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new UpdateUserInput
            {
                Id = id,
                Email = "email@test.com",
                Name = "New Name"
            };
            var validator = new UpdateUserValidator();

            //Act
            var response = validator.Validate(input);
            var responseErrors = response?.Errors.Select(error => error.ErrorMessage).ToArray();

            //Assert
            response.IsValid.Should().Be(isValid);

            foreach (var error in expectedErrors)
                responseErrors.Should().Contain(error);
        }

        [Theory]
        [InlineData("", "", false, "InvalidBody")]
        [InlineData(null, "", false, "InvalidBody")]
        [InlineData("  ", "", false, "InvalidBody")]
        [InlineData("", "   ", false, "InvalidBody")]
        [InlineData("  ", null, false, "InvalidBody")]
        [InlineData("emailtest.com", "New Name", false, "InvalidEmail")]
        [InlineData("email@test.com", "New Name", true)]
        public void Validator_ValidateInput_ShouldRespectValidations(string email, string name, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new UpdateUserInput
            {
                Id = "123123",
                Email = email,
                Name = name
            };
            var validator = new UpdateUserValidator();

            //Act
            var response = validator.Validate(input);
            var responseErrors = response?.Errors.Select(error => error.ErrorMessage).ToArray();

            //Assert
            response.IsValid.Should().Be(isValid);

            foreach (var error in expectedErrors)
                responseErrors.Should().Contain(error);
        }

        [Fact]
        public void Interactor_ValidCase_ShouldReturnSuccess()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var email = "email@test.com";
            var name = "New Name";
            var input = new UpdateUserInput
            {
                Id = id,
                Email = email,
                Name = name
            };
            var existingUser = new BlogUserEntity
            {
                Id = id,
                Enabled = true,
                Email = "oldEmail@test.com",
                Name = "Old Name"
            };
            var userWithEmail = new BlogUserEntity
            {
                Id = null
            };

            _validator.ValidateAsync(Arg.Any<UpdateUserInput>(), Arg.Any<CancellationToken>())
                      .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            _identityManager.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(userWithEmail));

            _identityManager.UpdateUserAsync(Arg.Any<string>(), Arg.Any<BlogUserEntity>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(true));

            var sut = GenerateInteractor();

            //Act
            var output = sut.UpdateAsync(input, CancellationToken.None).Result;

            //Assert
            output.Id.Should().Be(id);
            output.Email.Should().Be(email);
            output.Name.Should().Be(name);
        }

        private UpdateUserInteractor GenerateInteractor() => new UpdateUserInteractor(_logger, _validator, _identityManager);
    }
}
