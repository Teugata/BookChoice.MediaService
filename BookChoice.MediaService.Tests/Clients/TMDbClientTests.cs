using System.Net;
using AutoFixture.Xunit2;
using BookChoice.MediaService.Business.Clients.TMDb;
using BookChoice.MediaService.Data.Models.TMDb;
using BookChoice.MediaService.Tests.Attributes;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace BookChoice.MediaService.Tests.Clients
{
    public class TMDbClientTests
    {
        [Theory, AutoNSubstituteData]
        public void Constructor_ShouldThrow_WhenApiKeyMissing(
            HttpClient httpClient,
            [Frozen] IOptions<TMDbClientOptions> options)
        {
            // Arrange
            options.Value.Returns(new TMDbClientOptions { ApiKey = null });

            // Act
            Action act = () => new TMDbClient(httpClient, options);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Api Key must be provided*");
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ShouldReturnMovieWithImages_WhenApiReturnsSuccess(
            string id,
            Movie movie,
            Images images,
            VideosResult videosResult,
            [Frozen] IOptions<TMDbClientOptions> options)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/movie/{id}?api_key={options.Value.ApiKey}")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(movie));
            mockHttp.When($"*/movie/{id}/images?api_key={options.Value.ApiKey}")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(images));
            mockHttp.When($"*/movie/{id}/videos?api_key={options.Value.ApiKey}")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(videosResult));
            movie.Images = images;
            movie.Videos = videosResult.Results;

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost") 
            };
            var client = new TMDbClient(httpClient, options);

            // Act
            var result = await client.GetMovieAsync(id);

            // Assert
            result.Should().BeEquivalentTo(movie);
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ShouldReturnNull_WhenMovieNotFound(
            string id,
            [Frozen] IOptions<TMDbClientOptions> options)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/movie/{id}?api_key={options.Value.ApiKey}")
                .Respond(HttpStatusCode.NotFound);

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient, options);

            // Act
            var result = await client.GetMovieAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ShouldThrow_WhenHttpClientThrows(
            string id,
            [Frozen] IOptions<TMDbClientOptions> options)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/movie/{id}?api_key={options.Value.ApiKey}")
                .Throw(new HttpRequestException());

            var httpClient = new HttpClient(mockHttp);
            var client = new TMDbClient(httpClient, options);

            // Act
            Func<Task> act = async () => await client.GetMovieAsync(id);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
