using BookChoice.MediaService.Data.Models.TMDb;
using Google.Apis.YouTube.v3.Data;

namespace BookChoice.MediaService.Business.Services.Movies
{
    public interface IMovieService
    {
        Task<Movie?> GetAsync(string id, bool includeAdditionalYouTubeVideos, int maxYouTubeResults, CancellationToken cancellationToken);

        Task<MovieSearchResults?> SearchAsync(string query, int page, CancellationToken cancellationToken);
    }
}
