using FluentValidation;
using Mapster;
using TechBlog.Application.News.Create.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Domain.Gateways.Integrations.UsersApi;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.Create
{
    public sealed class CreateNewInteractor : ICreateNewUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<CreateNewInput> _validator;
        private readonly IBlogNewsRepository _repository;
        private readonly IUsersApi _usersApi;

        public CreateNewInteractor(
            ILoggerManager logger,
            IValidator<CreateNewInput> validator,
            IBlogNewsRepository repository,
            IUsersApi usersApi)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
            _usersApi = usersApi;
        }

        public async Task<CreateNewOutput> CreateAsync(CreateNewInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting creating the blog new", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            if(input.User.BlogUserType != BlogUserType.JOURNALIST)
                throw new ForbiddenException(ResponseMessage.UserMustBeAJournalist.ToString());

            if (!await _usersApi.ExistsByIdAsync(input.User.Id, cancellationToken))
                throw new BusinessException(ResponseMessage.UserDontExists.ToString());

            var blogNew = input.Adapt<BlogNewEntity>();

            blogNew.AuthorId = input.User.Id;

            blogNew = await _repository.AddAsync(blogNew, cancellationToken);

            _logger.Log("Blog new created", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            return blogNew.Adapt<CreateNewOutput>();
        }
    }
}
