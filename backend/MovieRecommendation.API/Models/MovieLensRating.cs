namespace MovieRecommendation.API.Models
{
    public class MovieLensRating
    {
        public int Id { get; set; }

        public int MovieLensUserId { get; set; }
        public int MovieLensMovieId { get; set; }

        public double Rating { get; set; }

        public long Timestamp { get; set; }
    }
}