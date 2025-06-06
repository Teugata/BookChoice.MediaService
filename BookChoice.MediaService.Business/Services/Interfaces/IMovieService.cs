using BookChoice.MediaService.Data.Models;

namespace BookChoice.MediaService.Business.Services
{
    public interface IMovieService
    {
        Task<Movie?> Get(string id);

        Task<IEnumerable<Movie>> Search(string title);
    }
}
