using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Auth {
    public class JwtService : IJwtService {
        private readonly JwtConfig _config;

        public JwtService(JwtConfig config) {
            _config = config;
        }

        public string GenerateSecurityToken(string id, string email, IEnumerable<string> roles, IEnumerable<Claim> userClaims) {
            var claims = new[] {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                new Claim("userId", id),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(",", roles))
            }.Concat(userClaims);

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.Secret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                expires: DateTime.UtcNow.AddMinutes(_config.ExpiresInMinutes),
                signingCredentials: signingCredentials,
                claims: claims);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken;
        }
    }
}
