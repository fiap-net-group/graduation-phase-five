using FluentValidation;
using System.Net.Mail;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.Update.Boundaries
{
    public sealed class UpdateUserValidator : AbstractValidator<UpdateUserInput>
    {
        public UpdateUserValidator()
        {
            RuleFor(input => input.Id)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidId.ToString());

            RuleFor(input => input)
                .Cascade(CascadeMode.Continue)
                .Custom((input, context) =>
                {
                    if(string.IsNullOrWhiteSpace(input.Email) && string.IsNullOrWhiteSpace(input.Name))
                    {
                        context.AddFailure(ResponseMessage.InvalidBody.ToString());
                        return;
                    }

                    if(!string.IsNullOrWhiteSpace(input.Email) && !MailAddress.TryCreate(input.Email, out _))
                        context.AddFailure(ResponseMessage.InvalidEmail.ToString());
                });
        }
    }
}
