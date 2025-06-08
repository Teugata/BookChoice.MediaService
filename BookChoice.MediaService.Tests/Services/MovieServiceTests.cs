using AutoFixture.Xunit2;
using BookChoice.MediaService.Business.Clients.TMDb;
using BookChoice.MediaService.Business.Clients.YouTube;
using BookChoice.MediaService.Business.Services.Movies;
using BookChoice.MediaService.Data.Models.TMDb;
using BookChoice.MediaService.Data.Models.YouTube;
using BookChoice.MediaService.Tests.Attributes;
using FluentAssertions;
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
            tmdbClient.GetMovieAsync(id).Returns<Task<Movie?>>(_ => throw exception);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            Func<Task> act = async () => await sut.GetAsync(id, true, 10);

            // Act
            await act.Should().ThrowAsync<Exception>().WithMessage(exception.Message);

            // Assert
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
            tmdbClient.GetMovieAsync(id).Returns((Movie?)null);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.GetAsync(id, true, 10);

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
            tmdbClient.GetMovieAsync(id).Returns(movie);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.GetAsync(id, true, 10);

            // Assert
            result.Should().Be(movie);
            logger.Received(1).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Any<Arg.AnyType>(),
                null,
                Arg.Any<Func<Arg.AnyType, Exception, string>>()!);
            await youTubeClient.DidNotReceive().SearchVideosAsync(Arg.Any<string>(), Arg.Any<int>());
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
            tmdbClient.GetMovieAsync(id).Returns(movie);
            youTubeClient.SearchVideosAsync(movie.Title, 1).Returns(youTubeResult);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.GetAsync(id, true, 1);

            // Assert
            result.Should().Be(movie);
            result.YouTubeVideos.Should().BeEquivalentTo(youTubeResult);
            await youTubeClient.Received(1).SearchVideosAsync(movie.Title, 1);
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
            tmdbClient.GetMovieAsync(id).Returns(movie);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.GetAsync(id, false, 5);

            // Assert
            result.Should().Be(movie);
            await youTubeClient.DidNotReceive().SearchVideosAsync(Arg.Any<string>(), Arg.Any<int>());
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ShouldReturnResults_WhenTmdbReturnsMovies(
            string query,
            IEnumerable<Movie> movies,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            tmdbClient.SearchMoviesAsync(query).Returns(movies);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.SearchAsync(query);

            // Assert
            result.Should().BeEquivalentTo(movies);
            await tmdbClient.Received(1).SearchMoviesAsync(query);
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ShouldReturnEmpty_WhenTmdbReturnsEmpty(
            string query,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            tmdbClient.SearchMoviesAsync(query).Returns(Enumerable.Empty<Movie>());
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            var result = await sut.SearchAsync(query);

            // Assert
            result.Should().BeEmpty();
            await tmdbClient.Received(1).SearchMoviesAsync(query);
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ShouldThrow_WhenTmdbThrows(
            string query,
            Exception exception,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            tmdbClient.SearchMoviesAsync(query).Returns<Task<IEnumerable<Movie>>>(_ => throw exception);
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            Func<Task> act = async () => await sut.SearchAsync(query);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(exception.Message);
        }
    }
}