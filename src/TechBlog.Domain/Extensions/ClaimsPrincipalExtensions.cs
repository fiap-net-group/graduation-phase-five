using System.Security.Claims;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Domain.Extensions
{
    public static class ClaimsExtensions
    {
        public const string NameClaimIdentifier = "name";
        public const string BlogUserTypeClaimIdentifier = "BlogUserType";

        public static string GetUserId(this ClaimsPrincipal userContext) => 
            userContext.GetClaim(ClaimTypes.NameIdentifier);

        public static (string Id, string Name, string Email, BlogUserType BlogUserType) GetBlogUserInformation(this ClaimsPrincipal userContext)
        {
            var id = userContext.GetClaim(ClaimTypes.NameIdentifier);
            var name = userContext.GetClaim(NameClaimIdentifier);
            var email = userContext.GetClaim(ClaimTypes.Email);

            if (!Enum.TryParse<BlogUserType>(userContext.GetClaim(BlogUserTypeClaimIdentifier), out var blogUserType))
                throw new ArgumentException("Invalid Jwt Token");

            return (id, name, email, blogUserType);
        }

        private static string GetClaim(this ClaimsPrincipal userContext, string claimIdentifier)
        {
            if (userContext == null)
                throw new ArgumentNullException(nameof(userContext));

            var claim = userContext.FindFirst(claimIdentifier);

            return claim?.Value;
        }
    }
}
