using Microsoft.AspNetCore.Mvc;
using MovieRecommendation.API.Data;
using MovieRecommendation.API.Services;
using Microsoft.EntityFrameworkCore;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TmdbService _tmdbService;
        public MovieController(
            AppDbContext context,
            TmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        //Film listeleme
        [HttpGet]
        public IActionResult GetAllMovies()
        {
            var movies = _context.Movies
        .AsNoTracking()
        .Where(x => x.PosterUrl != null)
        .OrderBy(x => x.Id)
        .Take(20)
        .Select(x => new
        {
            x.Id,
            x.Title,
            x.Genres,
            x.PosterUrl,
            x.VoteAverage
        })
        .ToList();

            return Ok(movies);
        }
        //Film arama
        [HttpGet("search")]
        public IActionResult SearchMovies(string query)
        {
            var movies = _context.Movies
                .Where(x => x.Title.Contains(query))
                .ToList();

            return Ok(movies);
        }
        //Film detay
        [HttpGet("{id}")]
        public IActionResult GetMovieById(int id)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == id);

            if (movie == null)
            {
                return NotFound("Film bulunamadı.");
            }

            return Ok(movie);
        }
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetMovieDetails(int id)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == id);

            if (movie == null)
            {
                return NotFound("Film bulunamadı.");
            }

            if (string.IsNullOrEmpty(movie.TmdbId))
            {
                return BadRequest("TMDB ID bulunamadı.");
            }

            var details = await _tmdbService
                .GetMovieDetails(int.Parse(movie.TmdbId));

            return Ok(details);
        }
    }
}