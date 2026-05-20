using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieRecommendation.API.Data;
using MovieRecommendation.API.Models;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WatchlistController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToWatchlist(int userId, int movieId)
        {
            var exists = _context.WatchlistItems
                .Any(x => x.UserId == userId && x.MovieId == movieId);

            if (exists)
            {
                return BadRequest("Film zaten watchlistte.");
            }

            var item = new WatchlistItem
            {
                UserId = userId,
                MovieId = movieId
            };

            _context.WatchlistItems.Add(item);

            _context.SaveChanges();

            return Ok("Watchliste eklendi.");
        }

        [HttpGet("{userId}")]
        public IActionResult GetUserWatchlist(int userId)
        {
            var movies = _context.WatchlistItems
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Movie)
                .Select(x => new
                {
                    x.Movie!.Id,
                    x.Movie.Title,
                    x.Movie.Genres,
                    x.Movie.PosterUrl,
                    x.Movie.VoteAverage,
                    x.Movie.Overview
                })
                .ToList();

            return Ok(movies);
        }
        [HttpDelete]
        public IActionResult RemoveFromWatchlist(int userId, int movieId)
        {
            var item = _context.WatchlistItems
                .FirstOrDefault(x => x.UserId == userId && x.MovieId == movieId);

            if (item == null)
            {
                return NotFound("Film watchlistte bulunamadı.");
            }

            _context.WatchlistItems.Remove(item);
            _context.SaveChanges();

            return Ok("Film watchlistten kaldırıldı.");
        }
    }
}