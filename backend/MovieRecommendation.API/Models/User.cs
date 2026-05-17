namespace MovieRecommendation.API.Models
{
    public class User
    {
        public int Id { get; set; }

        // Zorunlu alanlar
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Opsiyonel
        public string? FullName { get; set; }

        // Navigation
        public UserProfile? Profile { get; set; }
    }
}