using BookChoice.MediaService.Data.Models.TMDb;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace BookChoice.MediaService.Business.Clients.TMDb
{
    public class TMDbClient : ITMDbClient
    {
        private readonly HttpClient _httpClient;
        private readonly TMDbClientOptions _options;
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public TMDbClient(HttpClient httpClient, IOptions<TMDbClientOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            if (string.IsNullOrEmpty(_options.ApiKey))
            {
                throw new ArgumentException("Api Key must be provided for TMDbClient.", nameof(options));
            }
        }

        public async Task<Movie?> GetMovieAsync(string id)
        {
            var movie = await GetAsync<Movie>($"movie/{id}");
            if (movie != null)
            {
                movie.Images = await GetAsync<Images>($"movie/{id}/images");
                var videosResult = await GetAsync<VideosResult>($"movie/{id}/videos");
                movie.Videos = videosResult?.Results ?? [];
            }
            return movie;
        }

        private async Task<T?> GetAsync<T>(string path) where T : class
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    ["api_key"] = _options.ApiKey!
                };
                var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();

                var result = await _httpClient.GetAsync($"{path}?{queryString}");
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();
                var deserialized = JsonSerializer.Deserialize<T>(content, JsonSerializerOptions)
                    ?? throw new Exception($"Failed to deserialize {typeof(T).Name} data for path {path}.");

                return deserialized;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching {typeof(T).Name} with path {path}: {ex.Message}", ex);
            }
        }
    }
}
