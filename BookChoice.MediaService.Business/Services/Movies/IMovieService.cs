using BookChoice.MediaService.Data.Models.TMDb;

namespace BookChoice.MediaService.Business.Services.Movies
{
    public interface IMovieService
    {
        Task<Movie?> GetAsync(string id, bool includeAdditionalYouTubeVideos = true, int maxYouTubeResults = 10);

        Task<IEnumerable<Movie>> SearchAsync(string query);
    }
}
