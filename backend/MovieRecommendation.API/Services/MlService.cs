using System.Text;
using System.Text.Json;
using MovieRecommendation.API.DTOs;

namespace MovieRecommendation.API.Services
{
    public class MlRecommendationResult
    {
        public List<MlMovieDto> Recommendations { get; set; } = new();
    }

    public class MlMovieDto
    {
        public int MovieId { get; set; }
        public int? TmdbId { get; set; }
        public string Title { get; set; } = "";
        public string Genres { get; set; } = "";
        public double Score { get; set; }
    }

    public class MlRatingInput
    {
        public int MovieLensId { get; set; }
        public double Rating { get; set; }
    }

    public class MlService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public MlService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["MlService:BaseUrl"] ?? "http://localhost:8000";
        }

        public async Task<List<MlMovieDto>> GetRecommendationsByRatings(
            List<MlRatingInput> ratings, int n = 10)
        {
            try
            {
                var payload = new { ratings };
                var json = JsonSerializer.Serialize(payload,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync(
                    $"{_baseUrl}/recommend/by-ratings?n={n}", content);

                if (!response.IsSuccessStatusCode) return new();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<MlRecommendationResult>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Recommendations ?? new();
            }
            catch (Exception ex)
            {
                return new(); // ML servis çalışmıyorsa boş dön, fallback devreye girer
            }
        }
    }
}