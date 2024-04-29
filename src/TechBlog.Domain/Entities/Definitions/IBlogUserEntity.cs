using TechBlog.Domain.ValueObjects;

namespace TechBlog.Domain.Entities.Definitions
{
    public interface IBlogUserEntity<TId> : IBaseEntity<TId>
    {
        string Name { get; set; }
        string Email { get; set; }
        BlogUserType BlogUserType { get; set; }
        bool Enabled { get; set; }
    }
}
