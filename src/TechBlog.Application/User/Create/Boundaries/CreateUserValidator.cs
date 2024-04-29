using FluentValidation;
using System.Net.Mail;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.Create.Boundaries
{
    public sealed class CreateUserValidator : AbstractValidator<CreateUserInput>
    {
        public CreateUserValidator()
        {
            RuleFor(input => input.Email).Cascade(CascadeMode.Continue)
                                 .Custom((email, context) =>
                                 {
                                     if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out _))
                                         context.AddFailure(ResponseMessage.InvalidEmail.ToString());
                                 });

            RuleFor(input => input.Name).Cascade(CascadeMode.Continue)
                                .NotEmpty()
                                .WithMessage(ResponseMessage.InvalidName.ToString());

            RuleFor(input => input.Password).Cascade(CascadeMode.Stop)
                                    .NotEmpty()
                                    .WithMessage(ResponseMessage.InvalidPassword.ToString())
                                    .MinimumLength(6)
                                    .WithMessage(ResponseMessage.InvalidPassword.ToString());

            RuleFor(input => input.BlogUserType).Cascade(CascadeMode.Continue)
                                        .NotEmpty()
                                        .WithMessage(ResponseMessage.InvalidUserType.ToString());
        }
    }
}
