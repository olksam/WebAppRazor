using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using WebApi.DTOs.Auth;

namespace WebApi.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger) {
            _logger = logger;
        }

        //
        //[Authorize]
        [HttpGet("test")]
        public async Task<ActionResult> Test() {
            _logger.LogError(new ArgumentException("Invalid arguemnt", "argument"), "GET api/test -> It works! (200)");

            return Ok("It works!");
        }
    }
}
