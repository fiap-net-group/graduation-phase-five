using System.Diagnostics.CodeAnalysis;
using TechBlog.Domain.Entities;

namespace TechBlog.Application.Request.UpdateRequestStatus.Boundaries
{
    [ExcludeFromCodeCoverage]
    public sealed record UpdateRequestStatusInput(RequestEntity Value);
}
