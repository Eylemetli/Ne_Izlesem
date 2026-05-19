namespace MovieRecommendation.API.Models
{
    public class WatchlistItem
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int MovieId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Movie? Movie { get; set; }
    }
}