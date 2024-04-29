using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.ChangePassword.Boundaries
{
    public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordInput>
    {
        public ChangePasswordValidator()
        {
            RuleFor(input => input.Id)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidId.ToString());

            RuleFor(input => input.CurrentPassword)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidCurrentPassword.ToString());

            RuleFor(input => input.NewPassword)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidNewPassword.ToString())
                .MinimumLength(6)
                .WithMessage(ResponseMessage.InvalidNewPassword.ToString());

            RuleFor(input => input.NewPasswordConfirmation)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidNewPasswordConfirmation.ToString())
                .Equal(input => input.NewPassword)
                .WithMessage(ResponseMessage.PasswordsMustBeTheSame.ToString());
        }
    }
}
