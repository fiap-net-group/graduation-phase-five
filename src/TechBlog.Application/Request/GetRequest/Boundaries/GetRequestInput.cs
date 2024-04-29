using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace TechBlog.Application.Request.GetRequest.Boundaries
{
    [ExcludeFromCodeCoverage]
    public sealed record GetRequestInput([SwaggerParameter("The request Id")]Guid RequestId);
}