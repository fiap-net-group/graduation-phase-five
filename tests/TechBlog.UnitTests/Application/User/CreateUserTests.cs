using Bogus;
using FluentValidation;
using Mapster;
using NSubstitute;
using TechBlog.Application.User.Create;
using TechBlog.Application.User.Create.Boundaries;
using TechBlog.Application.User.GetById.Boundaries;
using TechBlog.Application.User.Reactivate;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Integrations.NotificationsApi;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;
using TechBlog.UnitTests.Fixtures;

namespace TechBlog.UnitTests.Application.User
{
    public sealed class CreateUserTests
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<CreateUserInput> _validator;
        private readonly IIdentityManager _identityManager;
        private readonly IReactivateUserUseCase _reactivateUserUseCase;
        private readonly INotificationsApi _notificationsApi;

        public CreateUserTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<CreateUserInput>>();
            _identityManager = Substitute.For<IIdentityManager>();
            _reactivateUserUseCase = Substitute.For<IReactivateUserUseCase>();
            _notificationsApi = Substitute.For<INotificationsApi>();
            MapperFixture.AddMapper(typeof(CreateUserMapper).Assembly);
        }

        [Theory]
        [InlineData(BlogUserType.JOURNALIST, false, true, null)]
        [InlineData(BlogUserType.JOURNALIST, true, false, ResponseMessage.UserAlreadyExists)]
        [InlineData(BlogUserType.JOURNALIST, false, false, ResponseMessage.ErrorCreatingUser)]
        [InlineData(BlogUserType.JOURNALIST, true, true, ResponseMessage.UserAlreadyExists)]
        [InlineData(BlogUserType.READER, false, true, null)]
        [InlineData(BlogUserType.READER, true, false, ResponseMessage.UserAlreadyExists)]
        [InlineData(BlogUserType.READER, false, false, ResponseMessage.ErrorCreatingUser)]
        [InlineData(BlogUserType.READER, true, true, ResponseMessage.UserAlreadyExists)]
        public async Task Interactor_ValidInput_CreateUser(BlogUserType blogUserType, bool emailAlreadyExists, bool createSuccess, ResponseMessage? errorResponseMessage = null)
        {
            try
            {
                // Arrange
                var input = new CreateUserInput("email@email.com", "Password@01", "Email", blogUserType);
                var cancellationToken = new CancellationToken();
                var user = new BlogUserEntity
                {
                    Id = emailAlreadyExists ? Guid.NewGuid().ToString() : null,
                    Enabled = emailAlreadyExists                    
                };

                _validator.ValidateAsync(Arg.Any<CreateUserInput>(), cancellationToken)
                    .Returns(new FluentValidation.Results.ValidationResult());

                _identityManager.GetByEmailAsync(input.Email, cancellationToken)
                    .Returns(Task.FromResult(user));

                _identityManager.CreateUserAsync(Arg.Any<BlogUserEntity>(), cancellationToken)
                    .Returns(createSuccess);

                var config = TypeAdapterConfig<AccessTokenModel, CreateUserOutput>
                    .NewConfig()
                    .ConstructUsing(source => new CreateUserOutput(source))
                    .Config;

                var accessToken = new AccessTokenModel("Bearer", Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(10), Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(60), Guid.NewGuid().ToString());

                accessToken.Adapt<CreateUserOutput>(config);

                _identityManager.AuthenticateAsync(Arg.Any<BlogUserEntity>(), input.Password, cancellationToken, Arg.Any<(string, string)[]>())
                    .Returns(accessToken);

                var sut = GenerateCreateUserInteractor();

                // Act
                var result = await sut.CreateAsync(input, cancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<CreateUserOutput>(result);
            }
            catch(Exception ex)
            {
                // Assert
                Assert.Equal(errorResponseMessage.ToString(), ex.Message);
            }
            
        }

        private CreateUserInteractor GenerateCreateUserInteractor()
        {
            return new CreateUserInteractor(_logger, _validator, _identityManager, _reactivateUserUseCase, _notificationsApi);
        }
    }
}
