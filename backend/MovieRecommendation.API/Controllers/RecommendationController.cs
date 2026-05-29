using Microsoft.AspNetCore.Mvc;
using MovieRecommendation.API.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MovieRecommendation.API.DTOs;
using MovieRecommendation.API.Services;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly MlService _mlService;

        public RecommendationController(AppDbContext context, MlService mlService)
        {
            _context = context;
            _mlService = mlService;
        }

        //Ana ekran önerilerini üretir
        [HttpGet("home/{userId}")]
        public async Task<IActionResult> GetHomeRecommendations(int userId)
        {
            var userRatings = _context.UserRatings
                .Where(x => x.UserId == userId && x.Rating >= 4)
                .ToList();

            // Kullanıcının hiç puanı yoksa popüler/yüksek puanlı filmler
            if (!userRatings.Any())
            {
                var popularMovies = _context.Movies
                    .OrderByDescending(x => x.AverageRating ?? 0)
                    .ThenByDescending(x => x.RatingCount)
                    .Take(10)
                    .ToList();

                return Ok(popularMovies);
            }

            var ratedMovieIds = userRatings
                .Select(x => x.MovieId)
                .ToList();

            var likedMovieIds = userRatings
                .Select(x => x.MovieId)
                .ToList();

            var likedGenres = _context.Movies
                .Where(x => likedMovieIds.Contains(x.Id))
                .Select(x => x.Genres)
                .ToList();

            var genreList = likedGenres
                .Where(x => !string.IsNullOrEmpty(x))
                .SelectMany(x => x!.Split('|'))
                .Distinct()
                .ToList();

            var recommendations = _context.Movies
                .Where(x => !ratedMovieIds.Contains(x.Id))
                .AsEnumerable()
                .Where(x => !string.IsNullOrEmpty(x.Genres) &&
                            x.Genres.Split('|').Any(g => genreList.Contains(g)))
                .Take(10)
                .ToList();

            var response = recommendations
            .Select(x => new MovieCardDto
            {
                Id = x.Id,
                Title = x.Title,
                Genres = x.Genres,
                PosterUrl = x.PosterUrl,
                VoteAverage = x.VoteAverage,
                Overview = x.Overview,
                ReleaseDate = x.ReleaseDate
            })
            .ToList();

            var mlRatings = _context.UserRatings
    .Where(x => x.UserId == userId)
    .Join(_context.Movies,
          ur => ur.MovieId,
          m => m.Id,
          (ur, m) => new MlRatingInput
          {
              MovieLensId = m.MovieLensId,
              Rating = ur.Rating
          })
    .Where(x => x.MovieLensId > 0)
    .ToList();

            if (mlRatings.Any())
            {
                var mlResults = await _mlService.GetRecommendationsByRatings(mlRatings);
                if (mlResults.Any())
                {
                    var tmdbIds = mlResults
                        .Where(x => x.TmdbId.HasValue)
                        .Select(x => x.TmdbId!.Value.ToString())
                        .ToList();

                    var matchedMovies = _context.Movies
                        .Where(x => x.TmdbId != null && tmdbIds.Contains(x.TmdbId))
                        .Select(x => new MovieCardDto
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Genres = x.Genres,
                            PosterUrl = x.PosterUrl,
                            VoteAverage = x.VoteAverage,
                            Overview = x.Overview,
                            ReleaseDate = x.ReleaseDate
                        })
                        .ToList();

                    if (matchedMovies.Any())
                        return Ok(matchedMovies);
                }
            }

            return Ok(response);
        }
        //“Bunu beğenenler şunları da beğendi” mantığıyla öneri üretir.
        [HttpGet("collaborative/{movieId}")]
        public IActionResult GetCollaborativeRecommendations(int movieId)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == movieId);

            if (movie == null)
            {
                return NotFound("Film bulunamadı.");
            }

            var similarUsers = _context.MovieLensRatings
                .Where(x => x.MovieLensMovieId == movie.MovieLensId && x.Rating >= 4)
                .Select(x => x.MovieLensUserId)
                .Distinct()
                .Take(100)
                .ToList();

            if (!similarUsers.Any())
            {
                return NotFound("Benzer kullanıcı bulunamadı.");
            }

            var recommendedMovieLensIds = _context.MovieLensRatings
                .Where(x => similarUsers.Contains(x.MovieLensUserId)
                            && x.MovieLensMovieId != movie.MovieLensId
                            && x.Rating >= 4)
                .GroupBy(x => x.MovieLensMovieId)
                .Select(g => new
                {
                    MovieLensId = g.Key,
                    Score = g.Count(),
                    AverageRating = g.Average(x => x.Rating)
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.AverageRating)
                .Take(10)
                .Select(x => x.MovieLensId)
                .ToList();

            var recommendations = _context.Movies
                .Where(x => recommendedMovieLensIds.Contains(x.MovieLensId))
                .ToList();

            var response = recommendations
            .Select(x => new MovieCardDto
            {
                Id = x.Id,
                Title = x.Title,
                Genres = x.Genres,
                PosterUrl = x.PosterUrl,
                VoteAverage = x.VoteAverage,
                Overview = x.Overview,
                ReleaseDate = x.ReleaseDate
            })
           .ToList();

            return Ok(response);
        }
        /*Bu filmlerin türlerine göre content-based öneri üretir.
        Aynı zamanda MovieLens verisine göre collaborative öneri üretir.*/
        [HttpGet("hybrid/{userId}")]
        public async Task<IActionResult> GetHybridRecommendations(int userId)
        {
            var likedRatings = _context.UserRatings
                .Where(x => x.UserId == userId && x.Rating >= 4)
                .ToList();

            if (!likedRatings.Any())
            {
                var popularMovies = _context.Movies
                    .OrderByDescending(x => x.AverageRating ?? 0)
                    .ThenByDescending(x => x.RatingCount)
                    .Take(10)
                    .ToList();

                return Ok(popularMovies);
            }

            var likedMovieIds = likedRatings.Select(x => x.MovieId).ToList();

            var likedMovies = _context.Movies
                .Where(x => likedMovieIds.Contains(x.Id))
                .ToList();

            var likedGenres = likedMovies
                .Where(x => !string.IsNullOrEmpty(x.Genres))
                .SelectMany(x => x.Genres!.Split('|'))
                .Distinct()
                .ToList();

            var contentBasedMovies = _context.Movies
                .Where(x => !likedMovieIds.Contains(x.Id))
                .AsEnumerable()
                .Where(x => !string.IsNullOrEmpty(x.Genres) &&
                            x.Genres.Split('|').Any(g => likedGenres.Contains(g)))
                .Take(10)
                .ToList();

            var likedMovieLensIds = likedMovies
                .Select(x => x.MovieLensId)
                .ToList();

            var similarUsers = _context.MovieLensRatings
                .Where(x => likedMovieLensIds.Contains(x.MovieLensMovieId) && x.Rating >= 4)
                .Select(x => x.MovieLensUserId)
                .Distinct()
                .Take(100)
                .ToList();

            var collaborativeMovieLensIds = _context.MovieLensRatings
                .Where(x => similarUsers.Contains(x.MovieLensUserId)
                            && !likedMovieLensIds.Contains(x.MovieLensMovieId)
                            && x.Rating >= 4)
                .GroupBy(x => x.MovieLensMovieId)
                .Select(g => new
                {
                    MovieLensId = g.Key,
                    Score = g.Count(),
                    AverageRating = g.Average(x => x.Rating)
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.AverageRating)
                .Take(10)
                .Select(x => x.MovieLensId)
                .ToList();

            var collaborativeMovies = _context.Movies
                .Where(x => collaborativeMovieLensIds.Contains(x.MovieLensId))
                .ToList();

            var hybridMovies = contentBasedMovies
                .Concat(collaborativeMovies)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .Take(10)
                .ToList();

            var response = hybridMovies
           .Select(x => new MovieCardDto
           {
               Id = x.Id,
               Title = x.Title,
               Genres = x.Genres,
               PosterUrl = x.PosterUrl,
               VoteAverage = x.VoteAverage,
               Overview = x.Overview,
               ReleaseDate = x.ReleaseDate
           })
          .ToList();

            return Ok(response);
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyRecommendations()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var likedRatings = _context.UserRatings
                .Where(x => x.UserId == userId && x.Rating >= 4)
                .ToList();

            if (!likedRatings.Any())
            {
                var profile = _context.UserProfiles
                    .FirstOrDefault(x => x.UserId == userId);

                if (profile != null &&
                    !string.IsNullOrEmpty(profile.FavoriteGenres))
                {
                    var favoriteGenres = profile.FavoriteGenres
                        .Split('|')
                        .ToList();

                    var genreRecommendations = _context.Movies
                        .AsEnumerable()
                        .Where(x => !string.IsNullOrEmpty(x.Genres) &&
                                    x.Genres.Split('|')
                                    .Any(g => favoriteGenres.Contains(g)))
                        .Take(10)
                        .ToList();

                    return Ok(genreRecommendations);
                }

                var popularMovies = _context.Movies
                    .OrderByDescending(x => x.AverageRating ?? 0)
                    .ThenByDescending(x => x.RatingCount)
                    .Take(10)
                    .ToList();

                return Ok(popularMovies);
            }
            // ML servisine kullanıcının puanlarını gönder
            // ML servisine kullanıcının puanlarını gönder
            var mlRatings = _context.UserRatings
                .Where(x => x.UserId == userId)
                .Join(_context.Movies,
                      ur => ur.MovieId,
                      m => m.Id,
                      (ur, m) => new MlRatingInput
                      {
                          MovieLensId = m.MovieLensId,
                          Rating = ur.Rating
                      })
                .Where(x => x.MovieLensId > 0)
                .ToList();

            if (mlRatings.Any())
            {
                var mlResults = await _mlService.GetRecommendationsByRatings(mlRatings);

                if (mlResults.Any())
                {
                    var tmdbIds = mlResults
                        .Where(x => x.TmdbId.HasValue)
                        .Select(x => x.TmdbId!.Value.ToString())
                        .ToList();

                    var matchedMovies = _context.Movies
                        .Where(x => x.TmdbId != null && tmdbIds.Contains(x.TmdbId))
                        .Select(x => new MovieCardDto
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Genres = x.Genres,
                            PosterUrl = x.PosterUrl,
                            VoteAverage = x.VoteAverage,
                            Overview = x.Overview,
                            ReleaseDate = x.ReleaseDate
                        })
                        .ToList();

                    if (matchedMovies.Any())
                        return Ok(matchedMovies);
                }
            }
            // ML sonuç vermediyse mevcut mantık devam eder...

            var likedMovieIds = likedRatings
                .Select(x => x.MovieId)
                .ToList();

            var likedGenres = _context.Movies
                .Where(x => likedMovieIds.Contains(x.Id))
                .ToList()
                .Where(x => !string.IsNullOrEmpty(x.Genres))
                .SelectMany(x => x.Genres!.Split('|'))
                .Distinct()
                .ToList();

            var recommendations = _context.Movies
                .Where(x => !likedMovieIds.Contains(x.Id))
                .AsEnumerable()
                .Where(x => !string.IsNullOrEmpty(x.Genres) &&
                            x.Genres.Split('|').Any(g => likedGenres.Contains(g)))
                .Take(10)
                .ToList();

            var response = recommendations
            .Select(x => new MovieCardDto
            {
                Id = x.Id,
                Title = x.Title,
                Genres = x.Genres,
                PosterUrl = x.PosterUrl,
                VoteAverage = x.VoteAverage,
                Overview = x.Overview,
                ReleaseDate = x.ReleaseDate
            })
           .ToList();

            return Ok(response);
        }
    }
}