using BookChoice.MediaService.Business.Services.Movies;
using BookChoice.MediaService.Data.Models.TMDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BookChoice.MediaService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController(ILogger<MoviesController> _logger, IMovieService _movieService, IMemoryCache _cache) : ControllerBase
    {
        /// <summary>
        /// Retrieves movie details by its TMDb or IMDb ID from TMDB (https://www.themoviedb.org/) 
        /// and if enabled searches YouTube (https://www.youtube.com) for additional videos based on the title.
        /// </summary>
        /// <param name="id">The TMDb or IMDb identifier of the movie.</param>
        /// <response code="200">Returns the movie details if found.</response>
        /// <response code="400">If the ID is null or empty.</response>
        /// <response code="404">If no movie is found with the given ID.</response>
        /// <returns>The movie details.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(string id, bool includeAdditionalYouTubeVideos = true, int maxYouTubeResults = 10)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID cannot be null or empty.");
            }

            // Redis cache could be used here if needed, but for simplicity, we use in-memory cache.
            string cacheKey = $"movie:{id}:yt:{includeAdditionalYouTubeVideos}:max:{maxYouTubeResults}";
            if (_cache.TryGetValue<Movie>(cacheKey, out var cachedMovie))
            {
                return Ok(cachedMovie);
            }

            Movie? movie;
            try
            {
                movie = await _movieService.GetAsync(id, includeAdditionalYouTubeVideos, maxYouTubeResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movie with ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }

            if (movie == null)
            {
                return NotFound();
            }

            // Cache duration can be made configurable
            _cache.Set(cacheKey, movie, TimeSpan.FromMinutes(10));

            return Ok(movie);
        }
    }
}
