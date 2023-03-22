using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using WebApi.DTOs.Auth;

namespace WebApi.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase {

        [Authorize]
        [HttpGet("test")]
        public async Task<ActionResult> Test() {
            return Ok("It works!");
        }
    }
}
