
using BookChoice.MediaService.Data.Models.YouTube;

namespace BookChoice.MediaService.Business.Clients.YouTube
{
    public interface IYouTubeClient
    {
        Task<IEnumerable<YouTubeVideo>> SearchVideosAsync(string query, int maxResults = 10);
    }
}