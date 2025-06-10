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
        public async Task GetMovieAsync_ThrowsArgumentException_WhenIdIsNullOrWhitespace(
            string id)
        {
            // Arrange
            var httpClient = new HttpClient();
            var client = new TMDbClient(httpClient);

            // Act
            Func<Task> act = async () => await client.GetMovieAsync(id, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Movie ID cannot be null or empty*");
        }

        [Theory, AutoNSubstituteData]
        public async Task GetMovieAsync_ReturnsMovieWithImagesAndVideos_WhenApiReturnsSuccess(
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
            var result = await client.GetMovieAsync(id, default);

            // Assert
            result.Should().BeEquivalentTo(movie);
        }

        [Theory, AutoNSubstituteData]
        public async Task GetMovieAsync_ReturnsNull_WhenMovieNotFound(
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
            var result = await client.GetMovieAsync(id, default);

            // Assert
            result.Should().BeNull();
        }

        [Theory, AutoNSubstituteData]
        public async Task GetMovieAsync_ThrowsException_WhenHttpClientThrows(
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
            Func<Task> act = async () => await client.GetMovieAsync(id, default);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchMoviesAsync_ReturnsResults_WhenApiReturnsResults(
            string query,
            int page,
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
            var result = await client.SearchMoviesAsync(query, page, default);

            // Assert
            result.Should().BeEquivalentTo(searchResult);
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchMoviesAsync_ReturnsEmptyResults_WhenNoResults(
            string query,
            int page)
        {
            // Arrange
            var emptyResult = new MovieSearchResults { Results = Array.Empty<Movie>() };
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"*/search/movie*")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(emptyResult));

            var httpClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://localhost")
            };
            var client = new TMDbClient(httpClient);

            // Act
            var searchResult = await client.SearchMoviesAsync(query, page, default);

            // Assert
            searchResult!.Results.Should().BeEmpty();
        }

        [Theory]
        [InlineAutoData(default!)]
        [InlineAutoData("")]
        [InlineAutoData("  ")]
        public async Task SearchMoviesAsync_ThrowsArgumentException_WhenQueryIsNullOrWhitespace(
            string query,
            int page)
        {
            // Arrange
            var httpClient = new HttpClient();
            var client = new TMDbClient(httpClient);

            // Act
            Func<Task> act = async () => await client.SearchMoviesAsync(query, page, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Movie query cannot be null or empty*");
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchMoviesAsync_ReturnsNull_WhenApiReturns404(
            string query,
            int page)
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
            var searchResult = await client.SearchMoviesAsync(query, page, default);

            // Assert
            searchResult.Should().BeNull();
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchMoviesAsync_ThrowsException_WhenHttpClientThrows(
            string query,
            int page)
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
            Func<Task> act = async () => await client.SearchMoviesAsync(query, page, default);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Theory]
        [InlineAutoData("query", 0)]
        [InlineAutoData("query", -1)]
        public async Task SearchMoviesAsync_ThrowsArgumentException_WhenPageIsLessThanOne(
            string query,
            int page)
        {
            // Arrange
            var httpClient = new HttpClient();
            var client = new TMDbClient(httpClient);

            // Act
            Func<Task> act = async () => await client.SearchMoviesAsync(query, page, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Page must be at least 1*");
        }
    }
}