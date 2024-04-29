using TechBlog.Domain.Entities;

namespace TechBlog.Application.Email.Send.Boundaries
{
    public sealed class SendEmailOutput
    {
        public RequestEntity Value { get; set; }
        public Guid RequestId { get; set; }

        public SendEmailOutput(RequestEntity value)
        {
            Value = value;
        }

        public void SetRequestId(Guid requestId)
        {
            RequestId = requestId;
            Value.Id = requestId.ToString();
        }
    }
}
