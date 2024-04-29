using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.GetById.Boundaries
{
    public sealed class GetUserByIdValidator : AbstractValidator<GetUserByIdInput>
    {
        public GetUserByIdValidator()
        {
            RuleFor(input => input.Id)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidId.ToString());
        }
    }
}
