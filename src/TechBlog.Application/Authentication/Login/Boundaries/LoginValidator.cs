using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.Authentication.Login.Boundaries
{
    public class LoginValidator : AbstractValidator<LoginInput>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Username).Cascade(CascadeMode.Continue)
                                    .NotEmpty()
                                    .WithMessage(ResponseMessage.InvalidEmail.ToString());

            RuleFor(l => l.Password).Cascade(CascadeMode.Continue)
                                    .NotEmpty()
                                    .WithMessage(ResponseMessage.InvalidPassword.ToString());
        }
    }
}
