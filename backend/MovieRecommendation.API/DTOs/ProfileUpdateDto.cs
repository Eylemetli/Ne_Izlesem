namespace MovieRecommendation.API.DTOs
{
    public class ProfileUpdateDto
    {
        public string? FullName { get; set; }
        public string? FavoriteGenres { get; set; }
        public string? LanguagePreference { get; set; }
        public string? LocalOrForeign { get; set; }
        public string? WatchingPurpose { get; set; }
    }
}