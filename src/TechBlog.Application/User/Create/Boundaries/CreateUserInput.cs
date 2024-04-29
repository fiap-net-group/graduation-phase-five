using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.Create.Boundaries
{
    public sealed record CreateUserInput(string Email, string Password, string Name, BlogUserType BlogUserType)
    {
        public CreateUserInput WithoutPassword() =>
           new(Email, null, Name, BlogUserType);
    }
}
