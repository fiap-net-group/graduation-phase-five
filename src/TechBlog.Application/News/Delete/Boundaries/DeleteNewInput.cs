using System.Text.Json.Serialization;
using TechBlog.Application.Common.Boundaries;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.Delete.Boundaries
{
    public sealed record DeleteNewInput(string Id)
    {
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public BlogUserPort User { get; set; }
        public void AddUser((string Id, string Name, string Email, BlogUserType BlogUserType) information)
        {
            User = new BlogUserPort
            {
                Id = information.Id,
                Name = information.Name,
                Email = information.Email,
                BlogUserType = information.BlogUserType
            };
        }
    }
}
