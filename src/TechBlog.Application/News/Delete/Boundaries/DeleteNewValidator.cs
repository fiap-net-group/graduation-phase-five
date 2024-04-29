using FluentValidation;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.Delete.Boundaries
{
    public sealed class DeleteNewValidator : AbstractValidator<DeleteNewInput>
    {
        public DeleteNewValidator()
        {
            RuleFor(c => c.Id).Cascade(CascadeMode.Continue)
                              .NotEmpty()
                              .WithMessage(ResponseMessage.InvalidId.ToString());
        }
    }
}
