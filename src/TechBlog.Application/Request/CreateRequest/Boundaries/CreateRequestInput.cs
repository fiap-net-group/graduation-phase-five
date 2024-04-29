using System.Diagnostics.CodeAnalysis;

namespace TechBlog.Application.Request.CreateRequest.Boundaries
{
    [ExcludeFromCodeCoverage]
    public sealed record CreateRequestInput(string OperationName, object Value);
}