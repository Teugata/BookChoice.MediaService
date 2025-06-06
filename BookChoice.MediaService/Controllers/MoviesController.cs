using BookChoice.MediaService.Business.Services;
using BookChoice.MediaService.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookChoice.MediaService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController(ILogger<MoviesController> _logger, IMovieService _movieService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a movie by its TMDb or IMDb ID.
        /// </summary>
        /// <param name="id">The TMDb or IMDb identifier of the movie.</param>
        /// <response code="200">Returns the movie details if found.</response>
        /// <response code="400">If the ID is null or empty.</response>
        /// <response code="404">If no movie is found with the given ID.</response>
        /// <returns>The movie details.</returns>
        [HttpGet("GetMovie")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID cannot be null or empty.");
            }

            Movie? movie;
            try
            {
                movie = await _movieService.Get(id);
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

            return Ok(movie);
        }
    }
}
