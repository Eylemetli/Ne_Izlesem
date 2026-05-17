using Microsoft.AspNetCore.Mvc;
using MovieRecommendation.API.Data;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovieController(AppDbContext context)
        {
            _context = context;
        }

        //Film listeleme
        [HttpGet]
        public IActionResult GetAllMovies()
        {
            var movies = _context.Movies.ToList();

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
    }
}