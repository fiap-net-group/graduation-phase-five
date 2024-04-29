namespace TechBlog.Application.Authentication.RefreshToken.Boundaries
{
    public sealed class RefreshTokenInput
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Id { get; set; }
        
        public string RefreshToken { get; set; }
    }
}
