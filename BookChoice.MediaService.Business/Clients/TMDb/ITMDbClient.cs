using BookChoice.MediaService.Data.Models.TMDb;

namespace BookChoice.MediaService.Business.Clients.TMDb
{
    public interface ITMDbClient
    {
        Task<Movie?> GetMovieAsync(string id);
        Task<IEnumerable<Movie>> SearchMoviesAsync(string title);
    }
}
