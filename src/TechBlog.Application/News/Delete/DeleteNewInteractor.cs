using FluentValidation;
using TechBlog.Application.News.Delete.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.Delete
{
    public class DeleteNewInteractor : IDeleteNewUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<DeleteNewInput> _validator;
        private readonly IBlogNewsRepository _repository;

        public DeleteNewInteractor(
            ILoggerManager logger, 
            IValidator<DeleteNewInput> validator,
            IBlogNewsRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        public async Task<DeleteNewOutput> DeleteAsync(DeleteNewInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting deleting the blog new", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var blogNew = await _repository.GetByIdAsync(input.Id, cancellationToken);

            if (!blogNew.Enabled)
                throw new NotFoundException();

            if (!blogNew.IsTheOwner(input.User.Id))
                throw new ForbiddenException(ResponseMessage.UserIsNotTheOwner.ToString());

            await _repository.DeleteAsync(input.Id, cancellationToken);

            _logger.Log("Blog new deleted", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            return new DeleteNewOutput();
        }
    }
}
