namespace MovieRecommendation.API.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public int MovieLensId { get; set; }
        public string Title { get; set; }
        public string? Genres { get; set; }

        public string? TmdbId { get; set; }
        public string? ImdbId { get; set; }

        public double? AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}