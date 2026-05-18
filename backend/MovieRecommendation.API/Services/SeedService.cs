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
        public void SeedMovieLinks()
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "DataSeed",
                "links.csv");

            using var reader = new StreamReader(path);

            using var csv = new CsvHelper.CsvReader(
                reader,
                System.Globalization.CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>().ToList();

            foreach (var item in records)
            {
                int movieLensId = int.Parse(item.movieId);

                var movie = _context.Movies
                    .FirstOrDefault(x => x.MovieLensId == movieLensId);

                if (movie == null)
                {
                    continue;
                }

                movie.ImdbId = item.imdbId;

                if (!string.IsNullOrEmpty(item.tmdbId))
                {
                    movie.TmdbId = item.tmdbId;
                }
            }

            _context.SaveChanges();
        }
        public async Task SyncTmdbData(TmdbService tmdbService)
        {
            var movies = _context.Movies
                .Where(x => x.TmdbId != null &&
                            x.PosterUrl == null)
                .Take(50)
                .ToList();

            foreach (var movie in movies)
            {
                try
                {
                    var details = await tmdbService
                        .GetMovieDetails(int.Parse(movie.TmdbId!));

                    if (details == null)
                    {
                        continue;
                    }

                    movie.PosterUrl = details.FullPosterUrl;
                    movie.Overview = details.Overview;
                    movie.ReleaseDate = details.ReleaseDate;
                    movie.VoteAverage = details.VoteAverage;

                    Console.WriteLine($"TMDB synced: {movie.Title}");
                }
                catch
                {
                    continue;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}