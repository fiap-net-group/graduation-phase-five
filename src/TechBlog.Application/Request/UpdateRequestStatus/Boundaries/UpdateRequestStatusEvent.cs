using System.Diagnostics.CodeAnalysis;
using TechBlog.Domain.Gateways.Event;

namespace TechBlog.Application.Request.UpdateRequestStatus.Boundaries
{
    [ExcludeFromCodeCoverage]
    public class UpdateRequestStatusEvent : BaseEvent<UpdateRequestStatusInput>
    {
        public override string OperationName => nameof(UpdateRequestStatusEvent);
    }
}
