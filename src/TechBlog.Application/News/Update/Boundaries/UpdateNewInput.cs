using TechBlog.Application.Common.Boundaries;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.Update.Boundaries
{
    public sealed class UpdateNewInput
    {
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }

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

        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }


    }
}
