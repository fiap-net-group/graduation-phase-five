using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.Delete.Boundaries
{
    public sealed class DeleteUserValidator : AbstractValidator<DeleteUserInput>
    {
        public DeleteUserValidator()
        {
            RuleFor(input => input.Id)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidId.ToString());
        }
    }
}
