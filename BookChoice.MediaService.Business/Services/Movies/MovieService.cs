using BookChoice.MediaService.Business.Clients.TMDb;
using BookChoice.MediaService.Business.Clients.YouTube;
using BookChoice.MediaService.Data.Models.TMDb;
using Microsoft.Extensions.Logging;

namespace BookChoice.MediaService.Business.Services.Movies
{
    public class MovieService(ILogger<MovieService> _logger, ITMDbClient _tmdbClient, IYouTubeClient _youTubeClient) : IMovieService
    {
        /// <inheritdoc />
        public async Task<Movie?> GetAsync(string id, bool includeAdditionalYouTubeVideos, int maxYouTubeResults, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Movie ID cannot be null or empty.", nameof(id));
            }

            Movie? movieResult;
            try
            {
                movieResult = await _tmdbClient.GetMovieAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie with ID {Id}", id);
                throw;
            }

            if (movieResult == null)
            {
                _logger.LogWarning("Movie with ID {Id} not found.", id);
                return movieResult;
            }

            if (string.IsNullOrEmpty(movieResult.Title))
            {
                _logger.LogWarning("Movie with ID {Id} has no title.", id);
                return movieResult;
            }

            if (includeAdditionalYouTubeVideos)
            {
                movieResult.YouTubeVideos = await _youTubeClient.SearchVideosAsync(movieResult.Title, maxYouTubeResults, cancellationToken);
            }

            return movieResult;
        }

        /// <inheritdoc />
        public async Task<MovieSearchResults?> SearchAsync(string query, int page, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Movie query cannot be null or empty.", nameof(query));
            }

            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page), "Page must be at least 1.");
            }

            MovieSearchResults? searchResults;
            try
            {
                searchResults = await _tmdbClient.SearchMoviesAsync(query, page, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for movies with query {Query}", query);
                throw;
            }

            if (searchResults == null || !searchResults.Results.Any())
            {
                _logger.LogWarning("No movie search results found for query {Query}.", query);
            }

            return searchResults;
        }
    }
}
