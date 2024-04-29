using TechBlog.Domain.Entities.Definitions;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Domain.Entities
{
    public sealed class BlogUserEntity : BaseEntity<string>, IBlogUserEntity<string>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public BlogUserType BlogUserType { get; set; }
        public bool Enabled { get; set; }

        public bool Exists => !string.IsNullOrWhiteSpace(Id);

        public bool IsActive => Exists && Enabled;

        public BlogUserEntity()
        {
        }

        public BlogUserEntity(bool valid)
        {
            if (!valid)
                Id = string.Empty;
        }

        public bool Update(string name, string email)
        {
            var nameUpdated = UpdateName(name);
            var emailUpdated = UpdateEmail(email);

            if (nameUpdated || emailUpdated)
            {
                LastUpdateAt = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        private bool UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            Name = name;

            return true;
        }

        private bool UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            Email = email;

            return true;
        }
    }
}
