using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.Reactivate.Boundaries
{
    public sealed class ReactivateUserValidator : AbstractValidator<ReactivateUserInput>
    {
        public ReactivateUserValidator()
        {
            RuleFor(input => input.Id)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidId.ToString());

            RuleFor(input => input.Password)
               .Cascade(CascadeMode.Continue)
               .NotEmpty()
               .WithMessage(ResponseMessage.InvalidPassword.ToString())
               .MinimumLength(6)
               .WithMessage(ResponseMessage.InvalidPassword.ToString());
        }
    }
}
