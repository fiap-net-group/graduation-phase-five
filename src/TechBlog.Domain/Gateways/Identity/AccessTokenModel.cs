namespace TechBlog.Domain.Gateways.Identity
{
    public class AccessTokenModel
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshExpires { get; set; }
        public string UserId { get; set; }

        public bool Valid() =>
                   !string.IsNullOrWhiteSpace(TokenType) &&
                   !string.IsNullOrWhiteSpace(AccessToken) &&
                   Expires != DateTime.MinValue &&
                   !string.IsNullOrWhiteSpace(UserId);

        public AccessTokenModel()
        {
            TokenType = string.Empty;
            AccessToken = string.Empty;
            Expires = DateTime.MinValue;
            UserId = string.Empty;
        }

        public AccessTokenModel(string tokenType, string accessToken, DateTime expires, string refreshToken, DateTime refreshExpires, string userId)
        {
            TokenType = tokenType;
            AccessToken = accessToken;
            Expires = expires;
            RefreshToken = refreshToken;
            RefreshExpires = refreshExpires;
            UserId = userId;
        }
    }
}
