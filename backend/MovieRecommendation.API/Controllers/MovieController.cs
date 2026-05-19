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
                .AsNoTracking()
                .Where(x => x.PosterUrl != null)
                .Where(x => x.Title.ToLower().Contains(query.ToLower()))
                .OrderBy(x => x.Title)
                .Take(50)
                .ToList()
                .GroupBy(x => x.Title)
                .Select(g => g.First())
                .Take(20)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Genres,
                    x.PosterUrl,
                    x.VoteAverage,
                    x.Overview
                })
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
        [HttpGet("filter")]
        public IActionResult FilterMovies(string? genre, double? minRating)
        {
            var query = _context.Movies
                .AsNoTracking()
                .Where(x => x.PosterUrl != null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(x => x.Genres != null && x.Genres.Contains(genre));
            }

            if (minRating.HasValue)
            {
                query = query.Where(x => x.VoteAverage >= minRating.Value);
            }

            var movies = query
                .OrderByDescending(x => x.VoteAverage)
                .Take(20)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Genres,
                    x.PosterUrl,
                    x.VoteAverage,
                    x.Overview
                })
                .ToList();

            return Ok(movies);
        }
    }
}