using FluentValidation;
using Mapster;
using TechBlog.Application.Email.Send.Boundaries;
using TechBlog.Domain.Gateways.Email;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Entities;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.Email.Send
{
    public sealed class SendEmailInteractor : ISendEmailUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IEmailManager _emailManager;
        private readonly IValidator<SendEmailInput> _validator;

        public SendEmailInteractor(
            ILoggerManager logger,
            IEmailManager emailManager,
            IValidator<SendEmailInput> validator)
        {
            _logger = logger;
            _emailManager = emailManager;
            _validator = validator;
        }

        public async Task<SendEmailOutput> SendAsync(SendEmailInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting email sending", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            var (isValid, output) = await _validator.ValidateInputAsync<SendEmailInput, SendEmailOutput>(input, _logger, cancellationToken);

            if (!isValid) return output;

            _logger.Log("Input is valid", LoggerManagerSeverity.DEBUG, (LoggingConstants.Input, input));

            if (!await _emailManager.SendAsync(input.Adapt<EmailData>(), cancellationToken))
            {
                _logger.Log("Error ocurred sending the email", LoggerManagerSeverity.WARNING, (LoggingConstants.Input, input));

                return new SendEmailOutput(new RequestEntity(ResponseMessage.Default, RequestStatus.InfrastructureError));
            }

            _logger.Log("Ending email sending", LoggerManagerSeverity.DEBUG, (LoggingConstants.Input, input));

            return new SendEmailOutput(new RequestEntity(ResponseMessage.Default, RequestStatus.Completed));
        }
    }
}
