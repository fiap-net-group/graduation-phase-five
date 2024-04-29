using MassTransit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Infrastructure.Identity.Configuration;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TechBlog.Infrastructure.Identity
{
    public class EntityFrameworkIdentityManager : IIdentityManager
    {
        private readonly UserManager<EntityFrameworkIdentityUser> _userManager;
        private readonly ILoggerManager _logger;

        private readonly string _audience;
        private readonly string _issuer;
        private readonly byte[] _key;
        private readonly int _accessTokenExpiresInMinutes;
        private readonly int _refreshTokenExpiresInMinutes;

        public EntityFrameworkIdentityManager(
            UserManager<EntityFrameworkIdentityUser> userManager,
            IConfiguration configuration,
            ILoggerManager logger)
        {
            _userManager = userManager;
            _logger = logger;

            _audience = configuration["Gateways:Identity:Jwt:Audience"];
            _issuer = configuration["Gateways:Identity:Jwt:Issuer"];
            _key = Encoding.UTF8.GetBytes(configuration["Gateways:Identity:Jwt:Key"]);

            _accessTokenExpiresInMinutes = configuration.GetValue("Gateways:Identity:Jwt:AccessTokenExpiresInMinutes", 300);
            _refreshTokenExpiresInMinutes = configuration.GetValue("Gateways:Identity:Jwt:RefreshTokenExpiresInMinutes", 3000);
        }

        public async Task<bool> CreateUserAsync(BlogUserEntity user, CancellationToken cancellationToken)
        {
            var identityUser = new EntityFrameworkIdentityUser(user);

            var result = await _userManager.CreateAsync(identityUser, user.Password);

            if (!result.Succeeded)
            {
                _logger.Log("Error creating user", LoggerManagerSeverity.INFORMATION);
                return false;
            }

            await _userManager.AddClaimsAsync(identityUser, new[]
            {
                new Claim("BlogUserType", Enum.GetName(user.BlogUserType))
            });

            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(string id, BlogUserEntity existingUser, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return false;

            user.Update(existingUser);

            var result = await _userManager.UpdateAsync(user);

            _logger.Log("End updating user", LoggerManagerSeverity.DEBUG, ("result", result));

            return result.Succeeded;
        }

        public async Task<bool> ReactivateAsync(BlogUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var existingUser = await _userManager.FindByIdAsync(user.Id);

            if (existingUser == null)
                return false;

            await _userManager.DeleteAsync(existingUser);

            user.Enabled = true;

            return await CreateUserAsync(user, cancellationToken);
        }

        public async Task<BlogUserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByNameAsync(email);

            user ??= await _userManager.FindByEmailAsync(email);

            return user is not null ? user.AsBlogUserEntity() : new BlogUserEntity(false);
        }

        public async Task<BlogUserEntity> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(id);

            return user is not null ? user.AsBlogUserEntity() : new BlogUserEntity(false);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return false;

            user.Enabled = false;

            return (await _userManager.UpdateAsync(user)).Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string id, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return false;

            return (await _userManager.ChangePasswordAsync(user, currentPassword, newPassword)).Succeeded;
        }

        public async Task<AccessTokenModel> AuthenticateAsync(BlogUserEntity user, string password, CancellationToken cancellationToken, params (string name, string value)[] customClaims)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var identityUser = await _userManager.FindByIdAsync(user.Id);

            if (!await ValidateLoginAsync(identityUser, password))
                return new AccessTokenModel();

            var claims = await GenerateClaimsByUserAsync(identityUser, cancellationToken, customClaims);

            _logger.Log("Valid login", LoggerManagerSeverity.DEBUG, (LoggingConstants.Username, user.Email));

            return await GenerateAccessTokenAsync(identityUser, new ClaimsIdentity(claims));
        }

        public async Task<AccessTokenModel> RefreshTokenAsync(string id, string refreshToken, CancellationToken cancellationToken, params (string name, string value)[] customClaims)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var identityUser = await _userManager.FindByIdAsync(id);

            if (identityUser == null || identityUser.RefreshToken != refreshToken || identityUser.RefreshTokenExpires <= DateTime.UtcNow)
                return new AccessTokenModel();

            var claims = await GenerateClaimsByUserAsync(identityUser, cancellationToken, customClaims);

            _logger.Log("Valid refresh token", LoggerManagerSeverity.DEBUG, (LoggingConstants.Username, identityUser.Email));

            return await GenerateAccessTokenAsync(identityUser, new ClaimsIdentity(claims));
        }

        private async Task<bool> ValidateLoginAsync(EntityFrameworkIdentityUser user, string password)
        {
            if (user is null)
            {
                _logger.Log("User don't exists", LoggerManagerSeverity.INFORMATION);

                return false;
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.Log("Invalid password", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Username, user.Email));

                return false;
            }

            return true;
        }

        private static long ToUnixEpochDate(DateTime date)
           => (long)Math.Round((date.ToUniversalTime() - DateTimeOffset.UnixEpoch).TotalSeconds);

        private async Task<IList<Claim>> GenerateClaimsByUserAsync(EntityFrameworkIdentityUser identityUser, CancellationToken cancellationToken, (string name, string value)[] customClaims)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var claims = await _userManager.GetClaimsAsync(identityUser);
            var userRoles = await _userManager.GetRolesAsync(identityUser);

            foreach (var (Name, Value) in customClaims)
                claims.Add(new Claim(Name, Value));

            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, identityUser.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, identityUser.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, identityUser.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
                claims.Add(new Claim("role", userRole));

            return claims;
        }

        private async Task<AccessTokenModel> GenerateAccessTokenAsync(EntityFrameworkIdentityUser user, ClaimsIdentity claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256),
                Audience = _audience,
                Issuer = _issuer,
                Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpiresInMinutes)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpires = DateTime.UtcNow.AddMinutes(_refreshTokenExpiresInMinutes);

            await _userManager.UpdateAsync(user);

            return new AccessTokenModel("Bearer", tokenHandler.WriteToken(token), tokenDescriptor.Expires.Value, user.RefreshToken, user.RefreshTokenExpires, user.Id);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var randomNumberGenerator = RandomNumberGenerator.Create();

            randomNumberGenerator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
