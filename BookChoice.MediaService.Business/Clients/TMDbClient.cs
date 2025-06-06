using BookChoice.MediaService.Data.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace BookChoice.MediaService.Business.Clients
{
    public class TMDbClient : ITMDbClient
    {
        private readonly HttpClient _httpClient;
        private readonly TMDbClientOptions _options;

        public TMDbClient(HttpClient httpClient, IOptions<TMDbClientOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            if (string.IsNullOrEmpty(_options.ApiKey))
            {
                throw new ArgumentException("Api Key must be provided for TMDbClient.", nameof(options));
            }
        }

        public async Task<Movie?> Get(string id)
        {
            try
            {
                var queryParams = new Dictionary<string, string>()
                {
                    ["api_key"] = _options.ApiKey!,
                };
                var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();

                var result = await _httpClient.GetAsync($"movie/{id}?{queryString}");
                if (result.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    return null;
                }

                result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var movie = JsonSerializer.Deserialize<Movie>(content, options);
                return movie;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching movie with ID {id}: {ex.Message}", ex);
            }
        }
    }
}
