using TechBlog.Domain.Entities;
using TechBlog.Domain.Entities.Definitions;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Infrastructure.Identity.Configuration
{
    public class EntityFrameworkIdentityUser : Microsoft.AspNetCore.Identity.IdentityUser, IBlogUserEntity<string>
    {
        public string Name { get; set; }
        public BlogUserType BlogUserType { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }

        //For EF Core
        public EntityFrameworkIdentityUser() { }

        public EntityFrameworkIdentityUser(BlogUserEntity source)
        {
            Id = source.Id;
            Email = source.Email;
            UserName = source.Email;
            EmailConfirmed = true;
            Name = source.Name;
            BlogUserType = source.BlogUserType;
            Enabled = source.Enabled;
            CreatedAt = source.CreatedAt;
            LastUpdateAt = source.LastUpdateAt;
        }

        public BlogUserEntity AsBlogUserEntity() => new()
        {
            Id = Id,
            Email = Email,
            Name = Name,
            Password = null,
            BlogUserType = BlogUserType,
            Enabled = Enabled,
            CreatedAt = CreatedAt,
            LastUpdateAt = LastUpdateAt,
        };

        public void Update(BlogUserEntity source)
        {
            LastUpdateAt = source.LastUpdateAt;
            Name = source.Name;
            Email = source.Email;
            UserName = source.Email;
        }
    }
}
