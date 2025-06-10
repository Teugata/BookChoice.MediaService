using BookChoice.MediaService.Data.Models.TMDb;

namespace BookChoice.MediaService.Business.Clients.TMDb
{
    public interface ITMDbClient
    {
        /// <summary>
        /// Retrieves a movie by its TMDb or IMDb ID.
        /// </summary>
        /// <param name="id">The TMDb or IMDb identifier of the movie.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>The movie details if found; otherwise, null.</returns>
        Task<Movie?> GetMovieAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// Searches for movies based on a query string and page number.
        /// </summary>
        /// <param name="title">The search query.</param>
        /// <param name="page">The page number for paginated results.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>The search results containing movies; otherwise, null.</returns>
        Task<MovieSearchResults?> SearchMoviesAsync(string title, int page, CancellationToken cancellationToken);
    }
}
