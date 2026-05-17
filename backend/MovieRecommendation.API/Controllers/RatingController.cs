using Microsoft.AspNetCore.Mvc;
using MovieRecommendation.API.Data;
using MovieRecommendation.API.Models;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RatingController(AppDbContext context)
        {
            _context = context;
        }

        /* Kullanıcıya ve filme göre puan kaydeder.
        Puan 1 ile 5 arasında mı kontrol eder.
        Kullanıcı var mı kontrol eder.
        Film var mı kontrol eder.
        Daha önce puan verdiyse puanı günceller.
        İlk kez puan veriyorsa yeni kayıt oluşturur.
        UserRatings tablosuna kayıt atar*/
        [HttpPost]
        public IActionResult RateMovie(int userId, int movieId, double rating)
        {
            if (rating < 1 || rating > 5)
            {
                return BadRequest("Puan 1 ile 5 arasında olmalıdır.");
            }

            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var movie = _context.Movies.FirstOrDefault(x => x.Id == movieId);
            if (movie == null)
            {
                return NotFound("Film bulunamadı.");
            }

            var existingRating = _context.UserRatings
                .FirstOrDefault(x => x.UserId == userId && x.MovieId == movieId);

            if (existingRating != null)
            {
                existingRating.Rating = rating;
            }
            else
            {
                var userRating = new UserRating
                {
                    UserId = userId,
                    MovieId = movieId,
                    Rating = rating
                };

                _context.UserRatings.Add(userRating);
            }

            _context.SaveChanges();

            return Ok("Puan kaydedildi.");
        }
    }
}