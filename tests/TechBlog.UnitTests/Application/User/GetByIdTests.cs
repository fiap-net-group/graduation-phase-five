using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TechBlog.Application.User.Delete.Boundaries;
using TechBlog.Application.User.GetById;
using TechBlog.Application.User.GetById.Boundaries;
using TechBlog.Application.User.Update.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.UnitTests.Fixtures;

namespace TechBlog.UnitTests.Application.User
{
    public sealed class GetByIdTests
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<GetUserByIdInput> _validator;
        private readonly IIdentityManager _identityManager;

        public GetByIdTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<GetUserByIdInput>>();
            _identityManager = Substitute.For<IIdentityManager>();
            MapperFixture.AddMapper(typeof(GetUserByIdMapper).Assembly);
        }

        [Theory]
        [InlineData("", false, "InvalidId")]
        [InlineData(null, false, "InvalidId")]
        [InlineData("  ", false, "InvalidId")]
        [InlineData("123123", true)]
        public void Validator_ValidateId_ShouldRespectValidations(string id, bool isValid, params string[] expectedErrors)
        {
            //Arrange
            var input = new GetUserByIdInput(id);
            var validator = new GetUserByIdValidator();

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
            var input = new GetUserByIdInput(id);
            var existingUser = new BlogUserEntity
            {
                Id = id,
                Enabled = true,
                Email = "email@test.com",
                Name = "Name Surname"
            };

            _validator.ValidateAsync(Arg.Any<GetUserByIdInput>(), Arg.Any<CancellationToken>())
                      .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            var sut = GenerateInteractor();

            //Act
            var output = sut.GetByIdAsync(input, CancellationToken.None).Result;

            //Assert
            output.Id.Should().Be(id);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void Interactor_InvalidCase_ShouldThrow(bool exists, bool enabled)
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var input = new GetUserByIdInput(id);
            var existingUser = new BlogUserEntity
            {
                Id = exists ? id : string.Empty,
                Enabled = enabled,
                Email = "email@test.com",
                Name = "Name Surname"
            };

            _validator.ValidateAsync(Arg.Any<GetUserByIdInput>(), Arg.Any<CancellationToken>())
                      .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _identityManager.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(existingUser));

            var sut = GenerateInteractor();

            //Act
            var act = async () => await sut.GetByIdAsync(input, CancellationToken.None);

            //Assert
            if (!exists || !enabled)
                act.Should().ThrowExactlyAsync<NotFoundException>();
        }

        private GetUserByIdInteractor GenerateInteractor() => new(_logger, _validator, _identityManager);
    }
}
