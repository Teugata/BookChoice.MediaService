using BookChoice.MediaService.Data.Models.YouTube;

namespace BookChoice.MediaService.Business.Clients.YouTube
{
    public interface IYouTubeClient
    {
        /// <summary>
        /// Searches YouTube for videos matching the specified query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="maxResults">The maximum number of results to return.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>A collection of YouTube videos matching the query.</returns>
        Task<IEnumerable<YouTubeVideo>> SearchVideosAsync(string query, int maxResults, CancellationToken cancellationToken);
    }
}