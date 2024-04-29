namespace TechBlog.Domain.Entities.Definitions
{
    public interface IBlogNewEntity<TId> : IBaseEntity<TId>
    {
        string Title { get; set; }
        string Description { get; set; }
        string Body { get; set; }
        string[] Tags { get; set; }
        bool Enabled { get; set; }
        string AuthorId { get; set; }
    }
}
