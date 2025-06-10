using AutoFixture.Xunit2;
using BookChoice.MediaService.Business.Clients.TMDb;
using BookChoice.MediaService.Business.Clients.YouTube;
using BookChoice.MediaService.Business.Services.Movies;
using BookChoice.MediaService.Data.Models.TMDb;
using BookChoice.MediaService.Data.Models.YouTube;
using BookChoice.MediaService.Tests.Attributes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BookChoice.MediaService.Tests.Services
{
    public class MovieServiceTests
    {
        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ShouldLogAndRethrow_WhenTmdbClientThrows(
            string id,
            Exception exception,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            tmdbClient.GetMovieAsync(id, Arg.Any<CancellationToken>()).Returns<Task<Movie?>>(_ => throw exception);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            Func<Task> act = async () => await sut.GetAsync(id, true, 10, default);

            // Act & Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(exception.Message);
            logger.Received(1).Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                exception,
                Arg.Any<Func<object, Exception, string>>()!);
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ShouldReturnNullAndLogWarning_WhenMovieNotFound(
            string id,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            tmdbClient.GetMovieAsync(id, Arg.Any<CancellationToken>()).Returns((Movie?)null);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.GetAsync(id, true, 10, default);

            // Assert
            result.Should().BeNull();
            logger.Received(1).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Any<Arg.AnyType>(),
                null,
                Arg.Any<Func<Arg.AnyType, Exception, string>>()!);
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ShouldReturnMovieAndLogWarning_WhenMovieHasNoTitle(
            string id,
            Movie movie,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            movie.Title = default;
            tmdbClient.GetMovieAsync(id, Arg.Any<CancellationToken>()).Returns(movie);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.GetAsync(id, true, 10, default);

            // Assert
            result.Should().Be(movie);
            logger.Received(1).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Any<Arg.AnyType>(),
                null,
                Arg.Any<Func<Arg.AnyType, Exception, string>>()!);
            await youTubeClient.DidNotReceive().SearchVideosAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ShouldReturnMovieAndSetYouTubeVideos_WhenMovieHasTitle(
            string id,
            Movie movie,
            string title,
            IEnumerable<YouTubeVideo> youTubeResult,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            movie.Title = title;
            tmdbClient.GetMovieAsync(id, Arg.Any<CancellationToken>()).Returns(movie);
            youTubeClient.SearchVideosAsync(movie.Title, 1, Arg.Any<CancellationToken>()).Returns(youTubeResult);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.GetAsync(id, true, 1, default);

            // Assert
            result.Should().Be(movie);
            result.YouTubeVideos.Should().BeEquivalentTo(youTubeResult);
            await youTubeClient.Received(1).SearchVideosAsync(movie.Title, 1, Arg.Any<CancellationToken>());
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ShouldNotCallYouTube_WhenIncludeAdditionalYouTubeVideosIsFalse(
            string id,
            Movie movie,
            string title,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            movie.Title = title;
            tmdbClient.GetMovieAsync(id, Arg.Any<CancellationToken>()).Returns(movie);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.GetAsync(id, false, 5, default);

            // Assert
            result.Should().Be(movie);
            await youTubeClient.DidNotReceive().SearchVideosAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ShouldReturnResults_WhenTmdbReturnsMovies(
            string query,
            int page,
            MovieSearchResults searchResults,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            tmdbClient.SearchMoviesAsync(query, page, Arg.Any<CancellationToken>()).Returns(searchResults);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.SearchAsync(query, page, default);

            // Assert
            result.Should().BeEquivalentTo(searchResults);
            await tmdbClient.Received(1).SearchMoviesAsync(query, page, Arg.Any<CancellationToken>());
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ShouldReturnNull_WhenTmdbReturnsNull(
            string query,
            int page,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            tmdbClient.SearchMoviesAsync(query, page, Arg.Any<CancellationToken>()).Returns((MovieSearchResults)default!);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.SearchAsync(query, page, default);

            // Assert
            result.Should().BeNull();
            await tmdbClient.Received(1).SearchMoviesAsync(query, page, Arg.Any<CancellationToken>());
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ShouldThrow_WhenTmdbThrows(
            string query,
            int page,
            Exception exception,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            tmdbClient.SearchMoviesAsync(query, page, Arg.Any<CancellationToken>()).Returns<Task<MovieSearchResults?>>(_ => throw exception);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act & Assert
            Func<Task> act = async () => await sut.SearchAsync(query, page, default);
            await act.Should().ThrowAsync<Exception>().WithMessage(exception.Message);
        }
    }
}