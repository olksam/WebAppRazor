using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace WebApi.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase {
        private readonly ILogger<TestController> _logger;
        private readonly IMemoryCache _memoryCache;

        public TestController(
            ILogger<TestController> logger,
            IMemoryCache memoryCache) {

            _logger = logger;
            _memoryCache = memoryCache;
        }


        // [Authorize]
        // [ResponseCache(Duration = 30)]
        [HttpGet("test")]
        public async Task<ActionResult> Test() {
            var userId = "user42";

            // _memoryCache.Remove($"orders_{userId}");

            if (_memoryCache.TryGetValue<string>($"orders_{userId}", out var cachedData)) {
                return Ok(cachedData);
            } else {
                await Task.Delay(3000);
                var data = "It works!";
                _memoryCache.Set("cached_data", data, new MemoryCacheEntryOptions {
                    Priority = CacheItemPriority.High
                });
                return Ok(data);
            }

            // _logger.LogError(new ArgumentException("Invalid arguemnt", "argument"), "GET api/test -> It works! (200)");
        }
    }
}
