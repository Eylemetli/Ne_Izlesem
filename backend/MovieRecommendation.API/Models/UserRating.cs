namespace MovieRecommendation.API.Models
{
    public class UserRating
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int MovieId { get; set; }

        public double Rating { get; set; } // 1-5 arası puan

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Movie Movie { get; set; }
    }
}