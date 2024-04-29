using FluentValidation;

namespace TechBlog.Application.Request.CreateRequest.Boundaries
{
    public sealed class CreateRequestValidator : AbstractValidator<CreateRequestInput>
    {
        public CreateRequestValidator()
        {
            RuleFor(request => request.OperationName).NotEmpty();

            RuleFor(request => request.Value).NotNull();
        }
    }
}