using BookChoice.MediaService.Business.Clients.TMDb;
using BookChoice.MediaService.Business.Clients.YouTube;
using BookChoice.MediaService.Data.Models.TMDb;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookChoice.MediaService.Business.Services.Movies
{
    public class MovieService(ILogger<MovieService> _logger, ITMDbClient _client, IYouTubeClient _youTubeClient) : IMovieService
    {
        public async Task<Movie?> GetAsync(string id, bool includeAdditionalYouTubeVideos = true, int maxYouTubeResults = 10)
        {
            Movie? movieResult;
            try
            {
                movieResult = await _client.GetMovieAsync(id);
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
                movieResult.YouTubeVideos = await _youTubeClient.SearchVideosAsync(movieResult.Title, maxYouTubeResults);
            }

            return movieResult;
        }

        public Task<IEnumerable<Movie>> SearchAsync(string title)
        {
            throw new NotImplementedException();
        }
    }
}
