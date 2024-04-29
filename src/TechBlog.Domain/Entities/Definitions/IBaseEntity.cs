namespace TechBlog.Domain.Entities.Definitions
{
    public interface IBaseEntity<TId>
    {
        TId Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime LastUpdateAt { get; set; }
    }
}
