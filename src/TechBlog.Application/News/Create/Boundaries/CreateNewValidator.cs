using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.Create.Boundaries
{
    public sealed class CreateNewValidator : AbstractValidator<CreateNewInput>
    {
        public CreateNewValidator()
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

            RuleFor(c => c.Tags).Cascade(CascadeMode.Continue)
                                .NotEmpty()
                                .WithMessage(ResponseMessage.InvalidTags.ToString());
        }
    }
}
