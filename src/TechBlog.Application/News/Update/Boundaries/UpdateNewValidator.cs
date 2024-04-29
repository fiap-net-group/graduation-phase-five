using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.Update.Boundaries
{
    public sealed class UpdateNewValidator : AbstractValidator<UpdateNewInput>
    {
        public UpdateNewValidator()
        {
            RuleFor(c => c.Title).Cascade(CascadeMode.Continue)
                                 .NotEmpty()
                                 .WithMessage(ResponseMessage.InvalidTitle.ToString());

            RuleFor(c => c.Description).Cascade(CascadeMode.Continue)
                                        .NotEmpty()
                                        .WithMessage(ResponseMessage.InvalidDescription.ToString());

            RuleFor(c => c.Body).Cascade(CascadeMode.Continue)
                                .NotEmpty()
                                .WithMessage(ResponseMessage.InvalidBody.ToString());
        }
    }
}
