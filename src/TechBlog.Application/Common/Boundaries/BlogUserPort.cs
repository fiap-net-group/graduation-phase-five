using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.Common.Boundaries
{
    public class BlogUserPort
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public BlogUserType BlogUserType { get; set; }
    }
}
