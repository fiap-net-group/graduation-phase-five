
namespace TechBlog.Application.User.Update.Boundaries
{
    public sealed class UpdateUserInput
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}
