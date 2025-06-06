using BookChoice.MediaService.Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookChoice.MediaService.Business.Extensions
{
    public static class MovieServiceExtensions
    {
        public static IServiceCollection AddMovieService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMovieService, MovieService>();
            services.AddTMDbClient(configuration);

            return services;
        }
    }
}
