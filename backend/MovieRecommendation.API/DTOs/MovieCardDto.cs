namespace MovieRecommendation.API.DTOs
{
    public class MovieCardDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Genres { get; set; }

        public string? PosterUrl { get; set; }

        public double? VoteAverage { get; set; }

        public string? Overview { get; set; }

        public string? ReleaseDate { get; set; }
    }
}