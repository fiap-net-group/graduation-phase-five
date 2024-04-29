using TechBlog.Domain.Gateways.Event;

namespace TechBlog.Application.Email.Send.Boundaries
{
    public sealed class SendEmailEvent : BaseEvent<SendEmailInput>
    {
        public override string OperationName => nameof(SendEmailEvent);
    }
}
