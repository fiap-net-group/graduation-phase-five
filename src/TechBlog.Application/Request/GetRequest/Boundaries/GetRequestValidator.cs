using FluentValidation;

namespace TechBlog.Application.Request.GetRequest.Boundaries
{
    public sealed class GetRequestValidator : AbstractValidator<GetRequestInput>
    {
        public GetRequestValidator()
        {
            RuleFor(request => request.RequestId).NotEmpty();
        }
    }
}
