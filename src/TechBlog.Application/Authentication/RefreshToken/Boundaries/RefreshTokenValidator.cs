using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.Authentication.RefreshToken.Boundaries
{
    public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenInput>
    {
        public RefreshTokenValidator()
        {
            RuleFor(input => input.Id)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidId.ToString());

            RuleFor(input => input.RefreshToken)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidCredentials.ToString());
        }
    }
}
