using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using TechBlog.Application.Email.Send;
using TechBlog.Application.Email.Send.Boundaries;
using TechBlog.Domain.Gateways.Email;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;
using TechBlog.UnitTests.Fixtures;

namespace TechBlog.UnitTests.Application.Email
{
    public sealed class SendEmailTests
    {
        private readonly ILoggerManager _logger;
        private readonly IEmailManager _emailManager;
        private readonly IValidator<SendEmailInput> _validator;

        public SendEmailTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _emailManager = Substitute.For<IEmailManager>();
            _validator = Substitute.For<IValidator<SendEmailInput>>();
            MapperFixture.AddMapper();
        }

        [Fact]
        public void Interactor_ValidInput_SholdSendEmail()
        {
            // Arrange
            var input = new SendEmailInput("email@email.com","Some Subject","Some Body");

            _validator.ValidateAsync(Arg.Any<SendEmailInput>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(new ValidationResult());
            _emailManager.SendAsync(Arg.Any<EmailData>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(true);

            var sut = GenerateInteractor();

            //Act
            var result = sut.SendAsync(input, CancellationToken.None).Result;

            //Assert
            result.Value.Should().NotBeNull();
            result.Value.Status.Should().Be(RequestStatus.Completed);
            result.Value.Message.Should().Be(ResponseMessage.Default);
        }

        [Theory]
        [InlineData("","Some Subject","Some Body", ResponseMessage.InvalidEmail)]
        [InlineData("email@email.com","","Some Body", ResponseMessage.InvalidEmailSubject)]
        [InlineData("email@email.com","Some Subject","", ResponseMessage.InvalidEmailBody)]
        public void Interactor_InvalidInput_ShouldReturnInvalid(string to, string subject, string body, ResponseMessage expectedMessage)
        {
            // Arrange
            var input = new SendEmailInput(to, subject, body);

            _validator.ValidateAsync(Arg.Any<SendEmailInput>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(new ValidationResult(new List<ValidationFailure>
                {
                    new("To", "To is required"),
                    new("Subject", "Subject is required"),
                    new("Body", "Body is required")
                }));

            var sut = GenerateInteractor(validator: new SendEmailValidator());

            //Act
            var result = sut.SendAsync(input, CancellationToken.None).Result;

            //Assert
            result.Value.Should().NotBeNull();
            result.Value.Status.Should().Be(RequestStatus.InvalidInformation);
            result.Value.Message.Should().Be(expectedMessage);
        }

        [Theory]
        [InlineData("", false, ResponseMessage.InvalidEmail)]
        [InlineData("email.com", false, ResponseMessage.InvalidEmail)]
        [InlineData("email@email.com", true)]
        [InlineData(" ", false, ResponseMessage.InvalidEmail)]
        public void Validator_To_ShouldRespectValidations(string to,bool expectedValid, ResponseMessage? expectedMessage = null)
        {
            // Arrange
            var input = new SendEmailInput(to, "Some Subject", "Some Body");
            var validator = new SendEmailValidator();

            //Act
            var result = validator.Validate(input);

            //Assert
            result.IsValid.Should().Be(expectedValid);
            if (expectedMessage != null)
                result.Errors.First().ErrorMessage.Should().Be(expectedMessage.ToString());
        }

        [Theory]
        [InlineData("", false, ResponseMessage.InvalidEmailSubject)]
        [InlineData(" ", false, ResponseMessage.InvalidEmailSubject)]
        [InlineData("Some Body", true)]
        public void validator_Subject_ShouldRespectValidations(string subject, bool expectedValid, ResponseMessage? expectedMessage = null)
        {
            // Arrange
            var input = new SendEmailInput("email@email.com", subject, "Some Body");
            var validator = new SendEmailValidator();

            //Act
            var result = validator.Validate(input);

            //Assert
            result.IsValid.Should().Be(expectedValid);
            if (expectedMessage != null)
                result.Errors.First().ErrorMessage.Should().Be(expectedMessage.ToString());
        }

        private SendEmailInteractor GenerateInteractor(
            ILoggerManager logger = null, 
            IEmailManager emailManager = null,
            IValidator<SendEmailInput> validator = null)
        {
            return new(logger ?? _logger, emailManager ?? _emailManager, validator ?? _validator);
        }
        
    }
}
