using BookChoice.MediaService.Data.Models.TMDb;
using System.Net;
using System.Text.Json;

namespace BookChoice.MediaService.Business.Clients.TMDb
{
    public class TMDbClient(HttpClient _httpClient) : ITMDbClient
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<Movie?> GetMovieAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Movie ID cannot be null or empty.", nameof(id));
            }

            var movie = await GetAsync<Movie>($"movie/{id}");
            if (movie != null)
            {
                movie.Images = await GetAsync<Images>($"movie/{id}/images");
                var videosResult = await GetAsync<VideosResult>($"movie/{id}/videos");
                movie.Videos = videosResult?.Results ?? [];
            }
            return movie;
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Movie query cannot be null or empty.", nameof(query));
            }

            var queryParams = new Dictionary<string, string>
            {
                ["query"] = Uri.EscapeDataString(query)
            };

            var result = await GetAsync<MovieSearchResults>("search/movie", queryParams);
            return result?.Results ?? [];
        }

        private async Task<T?> GetAsync<T>(string path, Dictionary<string, string>? queryParams = null) where T : class
        {
            try
            {
                string url = path;
                if (queryParams != null && queryParams.Count > 0)
                {
                    var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
                    url = $"{path}?{queryString}";
                }

                var result = await _httpClient.GetAsync(url);
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
