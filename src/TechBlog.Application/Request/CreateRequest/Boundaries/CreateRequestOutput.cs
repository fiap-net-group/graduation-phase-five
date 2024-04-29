using System.Diagnostics.CodeAnalysis;

namespace TechBlog.Application.Request.CreateRequest.Boundaries
{
    [ExcludeFromCodeCoverage]
    public sealed record CreateRequestOutput(Guid RequestId);
}