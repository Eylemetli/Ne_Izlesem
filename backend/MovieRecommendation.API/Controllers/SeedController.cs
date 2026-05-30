using Microsoft.AspNetCore.Mvc;
using MovieRecommendation.API.Services;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly SeedService _seedService;
        private readonly TmdbService _tmdbService;

        public SeedController(SeedService seedService, TmdbService tmdbService)
        {
            _seedService = seedService;
            _tmdbService = tmdbService;
        }

        [HttpPost("sync-tmdb")]
        public async Task<IActionResult> SyncTmdb()
        {
            await _seedService.SyncTmdbData(_tmdbService);
            return Ok("50 film senkronize edildi.");
        }
    }
}