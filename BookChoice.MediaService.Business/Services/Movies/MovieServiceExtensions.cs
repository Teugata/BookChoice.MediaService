using BookChoice.MediaService.Business.Clients.TMDb;
using BookChoice.MediaService.Business.Clients.YouTube;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookChoice.MediaService.Business.Services.Movies
{
    public static class MovieServiceExtensions
    {
        public static IServiceCollection AddMovieService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMovieService, MovieService>();
            services.AddTMDbClient(configuration);
            services.AddYouTubeClient(configuration);

            return services;
        }
    }
}
