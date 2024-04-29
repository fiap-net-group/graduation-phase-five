using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TechBlog.Application.Email.Send.Boundaries;
using TechBlog.Application.Request.CreateRequest;
using TechBlog.Application.Request.CreateRequest.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Event;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.Gateways.MemoryCache;

namespace TechBlog.UnitTests.Application.Request
{
    public sealed class CreateRequestTest
    {
        private readonly ILoggerManager _logger;
        private readonly IEventSenderManager _eventManager;
        private readonly IValidator<CreateRequestInput> _validator;
        private readonly IMemoryCacheManager _memoryCache;

        public CreateRequestTest()
        {
            _logger = Substitute.For<ILoggerManager>();
            _eventManager = Substitute.For<IEventSenderManager>();
            _validator = Substitute.For< IValidator<CreateRequestInput>>();
            _memoryCache = Substitute.For<IMemoryCacheManager>();
        }

        [Fact]
        public void Interactor_ValidInput_ShouldSendEvent()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var inputEvent = new SendEmailEvent();
            var input = new CreateRequestInput(inputEvent.OperationName, inputEvent);

            _validator.ValidateAsync(Arg.Any<CreateRequestInput>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ValidationResult()));
            _eventManager.SendAsync<SendEmailEvent, SendEmailInput>(Arg.Any<SendEmailEvent>(), Arg.Any<CancellationToken>())
                .Returns(requestId);

            var sut = GenerateInteractor();

            // Act
            var output = sut.CreateAsync<SendEmailEvent, SendEmailInput>(input, CancellationToken.None).Result;

            // Assert
            output.RequestId.Should().Be(requestId);
        
        }

        [Fact]
        public void Interactor_InvalidInput_ShouldBusinessException()
        {
            // Arrange
            var inputEvent = new SendEmailEvent();
            var input = new CreateRequestInput(inputEvent.OperationName, inputEvent);

            _validator.ValidateAsync(Arg.Any<CreateRequestInput>(), Arg.Any<CancellationToken>()).ThrowsAsync<BusinessException>();

            var sut = GenerateInteractor();

            // Act
            Func<Task> act = async () => await sut.CreateAsync<SendEmailEvent, SendEmailInput>(input, CancellationToken.None);

            // Assert
            act.Should().ThrowExactlyAsync<BusinessException>();
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("SendEmail", true)]
        [InlineData(" ", false)]
        public void Validator_OperationName_ShouldRespectValidations(string operationName,bool expectedValid)
        {
            //Arrange
            var input = new CreateRequestInput(operationName, new { Example = "Test" });
            var sut = new CreateRequestValidator();

            //Act
            var response = sut.Validate(input);

            //Assert
            Assert.Equal(expectedValid, response.IsValid);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void Validator_Value_ShouldRespectValidations(bool nullValue, bool expectedValid)
        {
            //Arrange
            var input = new CreateRequestInput("SendEmail", nullValue ? null : new { Example = "Test" });
            var sut = new CreateRequestValidator();

            //Act
            var response = sut.Validate(input);

            //Assert
            Assert.Equal(expectedValid, response.IsValid);
        }

        private CreateRequestInteractor GenerateInteractor() =>
            new(_logger, _eventManager, _validator, _memoryCache);
    }
}
