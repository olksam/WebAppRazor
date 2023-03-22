using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApi.Auth;
using WebApi.DTOs.Auth;
using WebApi.Entities;

namespace WebApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            IJwtService jwtService) {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        private async Task<AuthTokenDto> GenerateToken(AppUser user) {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var accessToken = _jwtService.GenerateSecurityToken(user.Email, roles, claims);

            var refreshToken = Guid.NewGuid().ToString("N").ToLower();

            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return new AuthTokenDto {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthTokenDto>> Register(RegisterRequest request) {
            var existingUser = await _userManager.FindByNameAsync(request.Email);

            if (existingUser is not null) {
                return Conflict("User already exists");
            }

            var user = new AppUser {
                UserName = request.Email,
                Email = request.Email,
                RefreshToken = Guid.NewGuid().ToString("N").ToLower()
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) {
                return BadRequest(result.Errors);
            }

            return await GenerateToken(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthTokenDto>> Login([FromBody] LoginRequest request) {
            var user = await _userManager.FindByNameAsync(request.Email);
            if (user is null) {
                return Unauthorized();
            }

            var canSignIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!canSignIn.Succeeded) {
                return Unauthorized();
            }

            return await GenerateToken(user);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthTokenDto>> RefreshToken([FromBody] RefreshTokenRequest request) {
            var user = await _userManager.Users.FirstOrDefaultAsync(e => e.RefreshToken == request.RefreshToken);

            if (user is null) {
                return Unauthorized();
            }

            return await GenerateToken(user);
        }
    }
}
