using MovieRecommendation.API.DTOs;
using System.Text.Json;

namespace MovieRecommendation.API.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<TmdbMovieDto?> GetMovieDetails(int tmdbId)
        {
            var token = _configuration["Tmdb:ApiKey"];

            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(
                "Authorization",
                $"Bearer {token}");

            var response = await _httpClient.GetAsync(
                $"https://api.themoviedb.org/3/movie/{tmdbId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbMovieDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result;
        }
    }
}