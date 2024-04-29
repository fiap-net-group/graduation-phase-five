using System.Security.Cryptography;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Domain.Entities.Definitions
{
    public interface IRequestEntity<TId> : IBaseEntity<TId>
    {
        ResponseMessage Message { get; set; }
        RequestStatus Status { get; set; }
    }
}
