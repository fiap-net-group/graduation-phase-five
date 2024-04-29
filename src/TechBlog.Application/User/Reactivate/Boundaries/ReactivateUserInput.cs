namespace TechBlog.Application.User.Reactivate.Boundaries
{
    public sealed record ReactivateUserInput(string Id, string Password)
    {
        public ReactivateUserInput WithoutPassword() => new(Id, null);
    }
}
