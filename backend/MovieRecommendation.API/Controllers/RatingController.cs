using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRecommendation.API.Data;
using MovieRecommendation.API.Models;
using System.Security.Claims;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RatingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult RateMovie(int movieId, double rating)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            if (rating < 1 || rating > 5)
                return BadRequest("Puan 1 ile 5 arasında olmalıdır.");

            var movie = _context.Movies.FirstOrDefault(x => x.Id == movieId);
            if (movie == null) return NotFound("Film bulunamadı.");

            var existingRating = _context.UserRatings
                .FirstOrDefault(x => x.UserId == userId && x.MovieId == movieId);

            if (existingRating != null)
            {
                existingRating.Rating = rating;
            }
            else
            {
                _context.UserRatings.Add(new UserRating
                {
                    UserId = userId,
                    MovieId = movieId,
                    Rating = rating
                });
            }

            _context.SaveChanges();
            return Ok("Puan kaydedildi.");
        }
    }
}