using TechBlog.Domain.Entities.Definitions;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Domain.Entities
{
    public sealed class RequestEntity : BaseEntity<string>, IRequestEntity<string>
    {
        public ResponseMessage Message { get; set; }
        public RequestStatus Status { get; set; }

        public RequestEntity() { }

        public RequestEntity(ResponseMessage message, RequestStatus status)
        {
            Message = message;
            Status = status;
        }
    }
}
