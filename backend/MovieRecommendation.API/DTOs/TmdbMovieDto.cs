using System.Text.Json.Serialization;
namespace MovieRecommendation.API.DTOs
{
    public class TmdbMovieDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
        public string? FullPosterUrl
        {
            get
            {
                if (string.IsNullOrEmpty(PosterPath))
                {
                    return null;
                }

                return $"https://image.tmdb.org/t/p/w500{PosterPath}";
            }
        }
    }
}