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
            if (string.IsNullOrWhiteSpace(query))
            {
                return Ok(new List<object>());
            }

            var search = query.Trim().ToLower();

            var movies = _context.Movies
                .AsNoTracking()
                .Where(x => x.Title.ToLower().Contains(search))
                .OrderBy(x => x.Title)
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
                return NotFound("Film bulunamadı.");

            // Poster yoksa TMDB'den çek
            if (movie.PosterUrl == null && movie.TmdbId != null)
            {
                try
                {
                    var details = await _tmdbService.GetMovieDetails(int.Parse(movie.TmdbId!));
                    if (details != null)
                    {
                        movie.PosterUrl = details.FullPosterUrl;
                        movie.Overview = details.Overview;
                        movie.ReleaseDate = details.ReleaseDate;
                        movie.VoteAverage = details.VoteAverage;
                        await _context.SaveChangesAsync();
                    }
                }
                catch { }
            }

            return Ok(new
            {
                movie.Id,
                movie.Title,
                movie.Genres,
                movie.PosterUrl,
                FullPosterUrl = movie.PosterUrl,
                movie.Overview,
                movie.ReleaseDate,
                movie.VoteAverage
            });
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
        [HttpGet("{id}/similar")]
        public IActionResult GetSimilarMovies(int id)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var genres = movie.Genres?.Split('|');

            var similarMovies = _context.Movies
                .AsNoTracking()
                .Where(x => x.Id != id)
                .Where(x => x.PosterUrl != null)
                .Where(x => genres!.Any(g => x.Genres!.Contains(g)))
                .OrderByDescending(x => x.VoteAverage)
                .Take(10)
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

            return Ok(similarMovies);
        }
        [HttpGet("genres")]
        public IActionResult GetGenres()
        {
            var genres = _context.Movies
                .Where(x => x.Genres != null)
                .AsEnumerable()
                .SelectMany(x => x.Genres!.Split('|'))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return Ok(genres);
        }
    }
}