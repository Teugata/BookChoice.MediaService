using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookChoice.MediaService.Business.Clients.YouTube
{
    public static class YouTubeClientExtensions
    {
        public static IServiceCollection AddYouTubeClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IYouTubeClient, YouTubeClient>();
            services.Configure<YouTubeClientOptions>(configuration.GetSection("YouTubeClient"));
            services.AddAutoMapper(typeof(YouTubeMappingProfile));

            return services;
        }
    }
}
