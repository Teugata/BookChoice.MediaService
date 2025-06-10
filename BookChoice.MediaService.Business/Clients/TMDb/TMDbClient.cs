using BookChoice.MediaService.Data.Models.TMDb;
using Google.Apis.YouTube.v3.Data;
using System.Net;
using System.Text.Json;

namespace BookChoice.MediaService.Business.Clients.TMDb
{
    public class TMDbClient(HttpClient _httpClient) : ITMDbClient
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<Movie?> GetMovieAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Movie ID cannot be null or empty.", nameof(id));
            }

            var movie = await GetAsync<Movie>($"movie/{id}", default, cancellationToken);
            if (movie != null)
            {
                movie.Images = await GetAsync<Images>($"movie/{id}/images", default, cancellationToken);
                var videosResult = await GetAsync<VideosResult>($"movie/{id}/videos", default, cancellationToken);
                movie.Videos = videosResult?.Results ?? [];
            }
            return movie;
        }

        public async Task<MovieSearchResults?> SearchMoviesAsync(string query, int page, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Movie query cannot be null or empty.", nameof(query));
            }

            var queryParams = new Dictionary<string, string>
            {
                ["query"] = Uri.EscapeDataString(query),
                ["page"] = page.ToString()
            };

            var result = await GetAsync<MovieSearchResults>("search/movie", queryParams, cancellationToken);
            return result;
        }

        private async Task<T?> GetAsync<T>(string path, Dictionary<string, string>? queryParams, CancellationToken cancellationToken) where T : class
        {
            try
            {
                string url = path;
                if (queryParams != null && queryParams.Count > 0)
                {
                    var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync(cancellationToken);
                    url = $"{path}?{queryString}";
                }

                var result = await _httpClient.GetAsync(url, cancellationToken);
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync(cancellationToken);
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
