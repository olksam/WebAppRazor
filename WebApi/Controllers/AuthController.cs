using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using WebApi.DTOs.Auth;

namespace WebApi.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {

        [HttpPost("login")]
        public async Task<ActionResult<AuthTokenDto>> Login([FromBody] LoginRequest request) {
            if (request is not {Login: "admin", Password: "pass123" }) {
                return Unauthorized();
            }

            var claims = new[] {
                new Claim(ClaimsIdentity.DefaultNameClaimType, "admin"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("super secret key"));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials,
                claims: claims);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthTokenDto {
                Token = tokenValue
            };
        }
    }
}
