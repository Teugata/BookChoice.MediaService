using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace BookChoice.MediaService.Business.Clients.TMDb
{
    public static class TMDbClientExtensions
    {
        public static IServiceCollection AddTMDbClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TMDbClientOptions>(configuration.GetSection("TMDbClient"));
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient<ITMDbClient, TMDbClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<TMDbClientOptions>>().Value;
                client.BaseAddress = options.BaseUrl ?? throw new InvalidOperationException("TMDbClient BaseUrl must be configured in appsettings.");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(retryPolicy);

            return services;
        }
    }
}
