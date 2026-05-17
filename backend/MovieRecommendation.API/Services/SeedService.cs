using CsvHelper;
using MovieRecommendation.API.Data;
using MovieRecommendation.API.Models;
using System.Globalization;

namespace MovieRecommendation.API.Services
{
    public class SeedService
    {
        private readonly AppDbContext _context;

        public SeedService(AppDbContext context)
        {
            _context = context;
        }

        public void SeedMovies()
        {
            if (_context.Movies.Any())
                return;

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "DataSeed",
                "movies.csv");

            using var reader = new StreamReader(path);

            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>().ToList();

            foreach (var item in records)
            {
                var movie = new Movie
                {
                    MovieLensId = int.Parse(item.movieId),
                    Title = item.title,
                    Genres = item.genres
                };

                _context.Movies.Add(movie);
            }

            _context.SaveChanges();
        }
        public void SeedRatings()
        {
            if (_context.MovieLensRatings.Any())
                return;

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "DataSeed",
                "ratings.csv");

            using var reader = new StreamReader(path);
            using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>().ToList();

            foreach (var item in records)
            {
                var rating = new MovieLensRating
                {
                    MovieLensUserId = int.Parse(item.userId),
                    MovieLensMovieId = int.Parse(item.movieId),
                    Rating = double.Parse(item.rating.ToString(), System.Globalization.CultureInfo.InvariantCulture),
                    Timestamp = long.Parse(item.timestamp)
                };

                _context.MovieLensRatings.Add(rating);
            }

            _context.SaveChanges();
        }
    }
}