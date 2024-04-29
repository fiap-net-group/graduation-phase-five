using FluentValidation;

namespace TechBlog.Application.Request.UpdateRequestStatus.Boundaries;

public class UpdateRequestStatusValidator : AbstractValidator<UpdateRequestStatusInput>
{
    public UpdateRequestStatusValidator()
    {
        RuleFor(request => request.Value.Id).NotEmpty();
    }
}
