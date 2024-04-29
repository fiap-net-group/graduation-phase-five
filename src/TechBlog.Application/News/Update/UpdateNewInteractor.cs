using FluentValidation;
using Mapster;
using TechBlog.Application.News.Update.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.Update
{
    public sealed class UpdateNewInteractor : IUpdateNewUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<UpdateNewInput> _validator;
        private readonly IBlogNewsRepository _repository;

        public UpdateNewInteractor(
            ILoggerManager logger, 
            IValidator<UpdateNewInput> validator, 
            IBlogNewsRepository repository)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
        }

        public async Task<UpdateNewOutput> UpdateAsync(UpdateNewInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting updating the blog new", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var blogNew = await _repository.GetByIdAsync(input.Id, cancellationToken);

            if (!blogNew.Enabled)
                throw new NotFoundException();

            if (!blogNew.IsTheOwner(input.User.Id))
                throw new ForbiddenException(ResponseMessage.UserIsNotTheOwner.ToString());

            blogNew.Update(input.Title, input.Description, input.Body);

            blogNew = await _repository.UpdateAsync(blogNew, cancellationToken);

            _logger.Log("Blog new updated", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            return blogNew.Adapt<UpdateNewOutput>();
        }
    }
}
