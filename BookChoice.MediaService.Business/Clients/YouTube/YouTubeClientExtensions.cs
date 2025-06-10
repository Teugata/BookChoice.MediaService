using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookChoice.MediaService.Business.Clients.YouTube
{
    public static class YouTubeClientExtensions
    {
        public static IServiceCollection AddYouTubeClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IYouTubeClient, YouTubeClient>();
            services.AddOptions<YouTubeClientOptions>()
                .Bind(configuration.GetSection("YouTubeClient"))
                .Validate(o => o.BaseUrl != null, "Base Url is required")
                .Validate(o => !string.IsNullOrEmpty(o.ApiKey), "Api Key is required");
            services.AddAutoMapper(typeof(YouTubeMappingProfile));

            return services;
        }
    }
}
