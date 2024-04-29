using TechBlog.Domain.Entities;

namespace TechBlog.Application.Common.Boundaries
{
    public class BlogNewPort
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public string[] Tags { get; set; }
        public BlogUserPort Author { get; set; }
        public DateTime LastUpdateAt { get; set; }

        public BlogNewPort()
        {
            
        }

        public BlogNewPort(BlogNewEntity entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            Body = entity.Body;
            Tags = entity.Tags;
            Author = new BlogUserPort
            {
                Id = entity.AuthorId,
            };
            LastUpdateAt = entity.LastUpdateAt;
        }
    }
}
