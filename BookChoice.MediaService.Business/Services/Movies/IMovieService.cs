using BookChoice.MediaService.Data.Models.TMDb;

namespace BookChoice.MediaService.Business.Services.Movies
{
    public interface IMovieService
    {
        /// <summary>
        /// Retrieves a movie by its TMDb or IMDb ID, optionally including additional YouTube videos.
        /// </summary>
        /// <param name="id">The TMDb or IMDb identifier of the movie.</param>
        /// <param name="includeAdditionalYouTubeVideos">If true, searches YouTube for additional videos related to the movie title.</param>
        /// <param name="maxYouTubeResults">The maximum number of YouTube videos to include if YouTube search is enabled.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>The movie details if found; otherwise, null.</returns>
        Task<Movie?> GetAsync(string id, bool includeAdditionalYouTubeVideos, int maxYouTubeResults, CancellationToken cancellationToken);

        /// <summary>
        /// Searches for movies based on a query string and page number.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="page">The page number for paginated results.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>The search results containing movies; otherwise, null.</returns>
        Task<MovieSearchResults?> SearchAsync(string query, int page, CancellationToken cancellationToken);
    }
}
