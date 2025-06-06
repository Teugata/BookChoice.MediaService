using BookChoice.MediaService.Data.Models;

namespace BookChoice.MediaService.Business.Clients
{
    public interface ITMDbClient
    {
        Task<Movie?> Get(string id);
    }
}
