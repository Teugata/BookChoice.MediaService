using AutoFixture.Xunit2;
using BookChoice.MediaService.Business.Clients.TMDb;
using BookChoice.MediaService.Data.Models.TMDb;
using BookChoice.MediaService.Tests.Attributes;
using FluentAssertions;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Net;

namespace BookChoice.MediaService.Tests.Clients
{
    public class TMDbClientTests
    {
        [Theory]
        [InlineAutoData(default!)]
        [InlineAutoData("")]
        [InlineAutoData("  ")]
        public async Task GetMovieAsync_ShouldThrow_WhenIdIsNullOrWhitespace(
            string id)
        {
            // Arrange
            var httpClient = new HttpClient();
            var client = new TMDbClient(httpClient);

            // Act
            Func<Task> act = async () => await client.GetMovieAsync(id);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Movie ID cannot be null or empty*");
        }

        [Theory, AutoNSubstituteData]
        public async Task GetMovieAsync_ShouldReturnMovieWithImagesAndVideos_WhenApiReturnsSuccess(
            string id,
            Movie movie,
            Images images,
            VideosResult videosResult)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/movie/{id}")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(movie));
            mockHttp.When($"*/movie/{id}/images")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(images));
            mockHttp.When($"*/movie/{id}/videos")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(videosResult));
            movie.Images = images;
            movie.Videos = videosResult.Results;

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient);

            // Act
            var result = await client.GetMovieAsync(id);

            // Assert
            result.Should().BeEquivalentTo(movie);
        }

        [Theory, AutoNSubstituteData]
        public async Task GetMovieAsync_ShouldReturnNull_WhenMovieNotFound(
            string id)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/movie/{id}")
                .Respond(HttpStatusCode.NotFound);

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient);

            // Act
            var result = await client.GetMovieAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Theory, AutoNSubstituteData]
        public async Task GetMovieAsync_ShouldThrow_WhenHttpClientThrows(
            string id)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/movie/{id}")
                .Throw(new HttpRequestException());

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient);

            // Act
            Func<Task> act = async () => await client.GetMovieAsync(id);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchMoviesAsync_ShouldReturnResults_WhenApiReturnsResults(
            string query,
            MovieSearchResults searchResult)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/search/movie*")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(searchResult));

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient);

            // Act
            var result = await client.SearchMoviesAsync(query);

            // Assert
            result.Should().BeEquivalentTo(searchResult.Results);
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchMoviesAsync_ShouldReturnEmpty_WhenNoResults(
            string query)
        {
            // Arrange
            var emptyResult = new MovieSearchResults { Results = [] };
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/search/movie*")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(emptyResult));

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient);

            // Act
            var result = await client.SearchMoviesAsync(query);

            // Assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineAutoData(default!)]
        [InlineAutoData("")]
        [InlineAutoData("  ")]
        public async Task SearchMoviesAsync_ShouldThrow_WhenQueryIsNullOrWhitespace(
            string query)
        {
            // Arrange
            var httpClient = new HttpClient();
            var client = new TMDbClient(httpClient);

            // Act
            Func<Task> act = async () => await client.SearchMoviesAsync(query);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Movie query cannot be null or empty*");
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchMoviesAsync_ShouldReturnEmpty_WhenApiReturns404(
            string query)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/search/movie*")
                .Respond(HttpStatusCode.NotFound);

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient);

            // Act
            var result = await client.SearchMoviesAsync(query);

            // Assert
            result.Should().BeEmpty();
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchMoviesAsync_ShouldThrow_WhenHttpClientThrows(
            string query)
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/search/movie*")
                .Throw(new HttpRequestException());

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient);

            // Act
            Func<Task> act = async () => await client.SearchMoviesAsync(query);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}