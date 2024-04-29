using TechBlog.Domain.Entities.Definitions;

namespace TechBlog.Domain.Entities
{
    public sealed class BlogNewEntity : BaseEntity<string>, IBlogNewEntity<string>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public string[] Tags { get; set; }
        public bool Enabled { get; set; }
        public string AuthorId { get; set; }

        public void Update(string title, string description, string body)
        {
            Title = title;
            Description = description;
            Body = body;
            LastUpdateAt = DateTime.UtcNow;
        }

        public bool IsTheOwner(string id)
        {
            return AuthorId == id;
        }
    }
}
