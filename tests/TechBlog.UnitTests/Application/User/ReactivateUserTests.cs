using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TechBlog.Application.User.Reactivate;
using TechBlog.Application.User.Reactivate.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;
using TechBlog.UnitTests.Fixtures;

namespace TechBlog.UnitTests.Application.User
{
    public sealed class ReactivateUserTests
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<ReactivateUserInput> _validator;
        private readonly IIdentityManager _identityManager;

        public ReactivateUserTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<ReactivateUserInput>>();
            _identityManager = Substitute.For<IIdentityManager>();
            MapperFixture.AddMapper(typeof(ReactivateUserMapper).Assembly);
        }

        [Theory]
        [InlineData("", false, "InvalidId")]
        [InlineData(null, false, "InvalidId")]
        [InlineData("  ", false, "InvalidId")]
        [InlineData("123123", true)]
        public void Validator_ValidateId_ShouldRespectValidations(string id, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new ReactivateUserInput(id, "newPassword@123");
            var validator = new ReactivateUserValidator();

            //Act
            var response = validator.Validate(input);
            var responseErrors = response?.Errors.Select(error => error.ErrorMessage).ToArray();

            //Assert
            response.IsValid.Should().Be(isValid);

            foreach (var error in expectedErrors)
                responseErrors.Should().Contain(error);
        }

        [Theory]
        [InlineData("", false, "InvalidPassword")]
        [InlineData(null, false, "InvalidPassword")]
        [InlineData("  ", false, "InvalidPassword")]
        [InlineData("Passw", false, "InvalidPassword")]
        [InlineData("Password@123", true)]
        public void Validator_ValidatePassword_ShouldRespectValidations(string password, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new ReactivateUserInput("123123", password);
            var validator = new ReactivateUserValidator();

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
            var input = new ReactivateUserInput(id, "NewPassword@123");

            var existingUser = new BlogUserEntity
            {
                Id = id,
                Enabled = false
            };
            var accessToken = new AccessTokenModel("Bearer", Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(10), Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(60), id);

            _validator.ValidateAsync(Arg.Any<ReactivateUserInput>(), Arg.Any<CancellationToken>())
                      .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            _identityManager.ReactivateAsync(Arg.Any<BlogUserEntity>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(true));

            _identityManager.AuthenticateAsync(Arg.Any<BlogUserEntity>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<(string, string)[]>())
                            .Returns(accessToken);

            var sut = GenerateInteractor();

            //Act
            var output = sut.ReactivateAsync(input, CancellationToken.None).Result;

            //Assert
            output.UserId.Should().Be(id);
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        public void Interactor_AllInvalidCases_ShouldThrow(bool userExixts, bool userEnabled, bool successReactivating)
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var input = new ReactivateUserInput("123123", "NewPassword@123");
            var existingUser = new BlogUserEntity
            {
                Id = userExixts ? id : null,
                Enabled = userEnabled
            };

            _validator.ValidateAsync(Arg.Any<ReactivateUserInput>(), Arg.Any<CancellationToken>())
                      .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            _identityManager.ReactivateAsync(Arg.Any<BlogUserEntity>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(successReactivating));

            var sut = GenerateInteractor();

            //Act
            var act = async () => await sut.ReactivateAsync(input, CancellationToken.None);

            //Assert
            if (!existingUser.IsActive)
                act.Should().ThrowExactlyAsync<NotFoundException>();

            if (existingUser.IsActive)
                act.Should().ThrowExactlyAsync<BusinessException>().WithMessage(ResponseMessage.UserAlreadyExists.ToString());

            if (!successReactivating)
                act.Should().ThrowExactlyAsync<InfrastructureException>();
        }

        private ReactivateUserInteractor GenerateInteractor() => new(_logger, _validator, _identityManager);
    }
}
