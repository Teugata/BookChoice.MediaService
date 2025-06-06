using BookChoice.MediaService.Business.Clients;
using BookChoice.MediaService.Data.Models;
using Microsoft.Extensions.Logging;

namespace BookChoice.MediaService.Business.Services
{
    public class MovieService(ILogger<MovieService> _logger, ITMDbClient _client) : IMovieService
    {
        public async Task<Movie?> Get(string id)
        {
            Movie? movieResult;
            try
            {
                movieResult = await _client.Get(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie with ID {Id}", id);
                throw;
            }

            if (movieResult == null)
            {
                _logger.LogWarning("Movie with ID {Id} not found.", id);
            }

            return movieResult;

        }

        public Task<IEnumerable<Movie>> Search(string title)
        {
            throw new NotImplementedException();
        }
    }
}
