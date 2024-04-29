using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TechBlog.Application.User.ChangePassword.Boundaries;
using TechBlog.Application.User.Delete;
using TechBlog.Application.User.Delete.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.UnitTests.Fixtures;

namespace TechBlog.UnitTests.Application.User
{
    public sealed class DeleteUserTests
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<DeleteUserInput> _validator;
        private readonly IIdentityManager _identityManager;

        public DeleteUserTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<DeleteUserInput>>();
            _identityManager = Substitute.For<IIdentityManager>();
        }

        [Theory]
        [InlineData("", false, "InvalidId")]
        [InlineData(null, false, "InvalidId")]
        [InlineData("  ", false, "InvalidId")]
        [InlineData("123123", true)]
        public void Validator_ValidateId_ShouldRespectValidations(string id, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new DeleteUserInput(id);
            var validator = new DeleteUserValidator();

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
            var input = new DeleteUserInput(id);
            var existingUser = new BlogUserEntity
            {
                Id = id,
                Enabled = true
            };

            _validator.ValidateAsync(Arg.Any<DeleteUserInput>(), Arg.Any<CancellationToken>())
                      .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            _identityManager.DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(true));

            var sut = GenerateInteractor();

            //Act
            var act = () => sut.DeleteAsync(input, CancellationToken.None);

            //Assert
            act.Should().NotThrowAsync();
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        public void Interactor_AllInvalidCases_ShouldThrow(bool userExixts, bool userEnabled, bool successDeleting)
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var input = new DeleteUserInput(id);
            var existingUser = new BlogUserEntity
            {
                Id = userExixts ? id : null,
                Enabled = userEnabled
            };

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            _identityManager.DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(successDeleting));


            var sut = GenerateInteractor();

            //Act
            var act = async () => await sut.DeleteAsync(input, CancellationToken.None);

            //Assert
            if (!existingUser.IsActive)
                act.Should().ThrowExactlyAsync<NotFoundException>();

            if (!successDeleting)
                act.Should().ThrowExactlyAsync<InfrastructureException>();
        }

        private DeleteUserInteractor GenerateInteractor() => new(_logger, _validator, _identityManager);
    }
}
