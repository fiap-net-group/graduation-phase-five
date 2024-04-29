using Bogus;
using FluentValidation;
using Mapster;
using NSubstitute;
using TechBlog.Application.Authentication.Login;
using TechBlog.Application.Authentication.Login.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.UnitTests.Application.Authentication
{
    public sealed class LoginTests
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<LoginInput> _validator;
        private readonly IIdentityManager _identityManager;

        public LoginTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<LoginInput>>();
            _identityManager = Substitute.For<IIdentityManager>();
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async void Interactor_ValidInput_LoginUser(bool userExists, bool accessTokenValid)
        {
            try
            {
                // Arrange
                var input = new LoginInput("username", "password");
                var cancellationToken = new CancellationToken();

                _validator
                    .ValidateAsync(Arg.Any<LoginInput>(), Arg.Any<CancellationToken>())
                    .Returns(new FluentValidation.Results.ValidationResult());

                _identityManager
                    .GetByEmailAsync(input.Username, cancellationToken)
                    .Returns(new Faker<BlogUserEntity>()
                        .RuleFor(c => c.Exists, userExists)
                        .RuleFor(c => c.Id, userExists ? Guid.NewGuid().ToString() : "")
                        .Generate());

                var accessToken = new Faker<AccessTokenModel>()
                    .RuleFor(c => c.AccessToken, accessTokenValid ? Guid.NewGuid().ToString() : "")
                    .RuleFor(c => c.Expires, accessTokenValid ? DateTime.Now.AddMinutes(10) : DateTime.Now.AddMinutes(-10))
                    .RuleFor(c => c.TokenType, accessTokenValid ? "TokenType" : "")
                    .RuleFor(c => c.UserId, accessTokenValid ? Guid.NewGuid().ToString() : "")
                    .Generate();

                var config = TypeAdapterConfig<AccessTokenModel,LoginOutput>
                    .NewConfig()
                    .ConstructUsing(source => new LoginOutput(source))
                    .Config;

                accessToken.Adapt<LoginOutput>(config);

                _identityManager
                    .AuthenticateAsync(Arg.Any<BlogUserEntity>(), input.Password, cancellationToken, Arg.Any<(string, string)>())
                    .Returns(accessToken);

                var sut = GenerateLoginInteractor();

                // Act
                var result = await sut.LoginAsync(input, cancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<LoginOutput>(result);
            }
            catch (Exception ex)
            {
                // Assert
                Assert.Equal(ResponseMessage.InvalidCredentials.ToString(), ex.Message);
            }
        }


        private LoginInteractor GenerateLoginInteractor()
        {
            return new LoginInteractor(_logger, _validator, _identityManager);
        }

    }
}
