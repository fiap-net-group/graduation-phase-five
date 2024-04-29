namespace TechBlog.Domain.Entities.Definitions
{
    public class BaseEntity<TId> : IBaseEntity<TId>
    {
        public TId Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
    }
}
