using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;

namespace BookChoice.MediaService.Business.Clients.TMDb
{
    public static class TMDbClientExtensions
    {
        public static IServiceCollection AddTMDbClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<TMDbClientOptions>()
                .Bind(configuration.GetSection("TMDbClient"))
                .Validate(o => o.BaseUrl != null, "Base Url is required")
                .Validate(o => !string.IsNullOrEmpty(o.AccessToken), "Access Token is required");
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient<ITMDbClient, TMDbClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<TMDbClientOptions>>().Value;
                client.BaseAddress = options.BaseUrl;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.AccessToken);
            }).AddPolicyHandler(retryPolicy);

            return services;
        }
    }
}
