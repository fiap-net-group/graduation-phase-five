using System.Diagnostics.CodeAnalysis;

namespace TechBlog.Domain.Gateways.Event
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseEvent<T>
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public abstract string OperationName { get; }
        public Guid RequestId { get; set; }
        public T Value { get; set; }
    }
}
