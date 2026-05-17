namespace MovieRecommendation.API.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? FavoriteGenres { get; set; } // "Action,Comedy"
        public string? LanguagePreference { get; set; }
        public string? LocalOrForeign { get; set; }
        public string? WatchingPurpose { get; set; }

        // Navigation
        public User User { get; set; }
    }
}