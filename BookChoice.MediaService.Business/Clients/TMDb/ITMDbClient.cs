using BookChoice.MediaService.Data.Models.TMDb;

namespace BookChoice.MediaService.Business.Clients.TMDb
{
    public interface ITMDbClient
    {
        Task<Movie?> GetMovieAsync(string id, CancellationToken cancellationToken);
        Task<MovieSearchResults?> SearchMoviesAsync(string title, int page, CancellationToken cancellationToken);
    }
}
