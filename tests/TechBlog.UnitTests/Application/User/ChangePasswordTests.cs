using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TechBlog.Application.User.ChangePassword;
using TechBlog.Application.User.ChangePassword.Boundaries;
using TechBlog.Application.User.Reactivate.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.UnitTests.Fixtures;

namespace TechBlog.UnitTests.Application.User
{
    public sealed class ChangePasswordTests
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<ChangePasswordInput> _validator;
        private readonly IIdentityManager _identityManager;

        public ChangePasswordTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<ChangePasswordInput>>();
            _identityManager = Substitute.For<IIdentityManager>();
            MapperFixture.AddMapper(typeof(ChangePasswordMapper).Assembly);
        }

        [Theory]
        [InlineData("", false, "InvalidId")]
        [InlineData(null, false, "InvalidId")]
        [InlineData("  ", false, "InvalidId")]
        [InlineData("123123", true)]
        public void Validator_ValidateId_ShouldRespectValidations(string id, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new ChangePasswordInput
            {
                Id = id,
                CurrentPassword = "oldPassword123",
                NewPassword = "newPassword123",
                NewPasswordConfirmation = "newPassword123"
            };
            var validator = new ChangePasswordValidator();

            //Act
            var response = validator.Validate(input);
            var responseErrors = response?.Errors.Select(error => error.ErrorMessage).ToArray();

            //Assert
            response.IsValid.Should().Be(isValid);

            foreach (var error in expectedErrors)            
                responseErrors.Should().Contain(error);            
        }

        [Theory]
        [InlineData("", false, "InvalidCurrentPassword")]
        [InlineData(null, false, "InvalidCurrentPassword")]
        [InlineData("  ", false, "InvalidCurrentPassword")]
        [InlineData("Password@123", true)]
        public void Validator_ValidateCurrentPassword_ShouldRespectValidations(string currentPassword, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new ChangePasswordInput
            {
                Id = "123123",
                CurrentPassword = currentPassword,
                NewPassword = "newPassword123",
                NewPasswordConfirmation = "newPassword123"
            };
            var validator = new ChangePasswordValidator();

            //Act
            var response = validator.Validate(input);
            var responseErrors = response?.Errors.Select(error => error.ErrorMessage).ToArray();

            //Assert
            response.IsValid.Should().Be(isValid);

            foreach (var error in expectedErrors)
                responseErrors.Should().Contain(error);
        }

        [Theory]
        [InlineData("", false, "InvalidNewPassword")]
        [InlineData(null, false, "InvalidNewPassword")]
        [InlineData("  ", false, "InvalidNewPassword")]
        [InlineData("newPa", false, "InvalidNewPassword")]
        [InlineData("newPassword123", true)]
        public void Validator_ValidateInvalidNewPassword_ShouldRespectValidations(string newPassword, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new ChangePasswordInput
            {
                Id = "123123",
                CurrentPassword = "Password@123",
                NewPassword = newPassword,
                NewPasswordConfirmation = "newPassword123"
            };
            var validator = new ChangePasswordValidator();

            //Act
            var response = validator.Validate(input);
            var responseErrors = response?.Errors.Select(error => error.ErrorMessage).ToArray();

            //Assert
            response.IsValid.Should().Be(isValid);

            foreach (var error in expectedErrors)
                responseErrors.Should().Contain(error);
        }

        [Theory]
        [InlineData("", false, "InvalidNewPasswordConfirmation")]
        [InlineData(null, false, "InvalidNewPasswordConfirmation")]
        [InlineData("  ", false, "InvalidNewPasswordConfirmation")]
        [InlineData("newPassword12", false, "PasswordsMustBeTheSame")]
        [InlineData("newPassword123", true)]
        public void Validator_ValidateInvalidNewPasswordConfirmation_ShouldRespectValidations(string newPassword, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new ChangePasswordInput
            {
                Id = "123123",
                CurrentPassword = "Password@123",
                NewPassword = "newPassword123",
                NewPasswordConfirmation = newPassword
            };
            var validator = new ChangePasswordValidator();

            //Act
            var response = validator.Validate(input);
            var responseErrors = response?.Errors.Select(error => error.ErrorMessage).ToArray();

            //Assert
            response.IsValid.Should().Be(isValid);

            foreach (var error in expectedErrors)
                responseErrors.Should().Contain(error);
        }

        [Fact]
        public void Interactor_ValidCase_ShouldReturnExpected()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var input = new ChangePasswordInput
            {
                Id = id,
                CurrentPassword = "Password@123",
                NewPassword = "newPassword123",
                NewPasswordConfirmation = "newPassword123"
            };

            var existingUser = new BlogUserEntity
            {
                Id = id,
                Enabled = true
            };

            _validator.ValidateAsync(Arg.Any<ChangePasswordInput>(), Arg.Any<CancellationToken>())
                      .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            _identityManager.ChangePasswordAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(true));

            var sut = GenerateInteractor();

            //Act
            var output = sut.ChangePasswordAsync(input, CancellationToken.None).Result;

            //Assert
            output.Id.Should().Be(input.Id);
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        public void Interactor_AllInvalidCases_ShouldThrow(bool userExixts, bool userEnabled, bool successChanging)
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var input = new ChangePasswordInput
            {
                Id = id,
                CurrentPassword = "Password@123",
                NewPassword = "newPassword123",
                NewPasswordConfirmation = "newPassword123"
            };

            var existingUser = new BlogUserEntity
            {
                Id = userExixts ? id : null,
                Enabled = userEnabled
            };

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            _identityManager.ChangePasswordAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(successChanging));


            var sut = GenerateInteractor();

            //Act
            var act = async () => await sut.ChangePasswordAsync(input, CancellationToken.None);

            //Assert
            if(!existingUser.IsActive)            
                act.Should().ThrowExactlyAsync<NotFoundException>();

            if(!successChanging)
                act.Should().ThrowExactlyAsync<InfrastructureException>();
        }

        private ChangePasswordInteractor GenerateInteractor() => new(_logger, _validator, _identityManager);
    }
}
