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
        [Theory]
        [InlineAutoNSubstituteData(default!)]
        [InlineAutoNSubstituteData("")]
        [InlineAutoNSubstituteData(" ")]
        public async Task GetAsync_ThrowsArgumentException_WhenIdIsNullOrWhitespace(
            string id,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            Func<Task> act = async () => await sut.GetAsync(id, true, 10, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Movie ID cannot be null or empty*");
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ThrowsAndLogsError_WhenTmdbClientThrows(
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
        public async Task GetAsync_ReturnsNullAndLogsWarning_WhenMovieNotFound(
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
        public async Task GetAsync_ReturnsMovieAndLogsWarning_WhenMovieHasNoTitle(
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
        public async Task GetAsync_ReturnsMovieAndSetsYouTubeVideos_WhenMovieHasTitle(
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
        public async Task GetAsync_DoesNotCallYouTube_WhenIncludeAdditionalYouTubeVideosIsFalse(
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
        public async Task SearchAsync_ReturnsResults_WhenTmdbReturnsMovies(
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
        public async Task SearchAsync_ReturnsNull_WhenTmdbReturnsNull(
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
        public async Task SearchAsync_ThrowsException_WhenTmdbThrows(
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

        [Theory]
        [InlineAutoNSubstituteData(0)]
        [InlineAutoNSubstituteData(-1)]
        public async Task SearchAsync_ThrowsArgumentOutOfRangeException_WhenPageIsLessThanOne(
            int page,
            string query,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            Func<Task> act = async () => await sut.SearchAsync(query, page, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*Page must be at least 1*");
        }

        [Theory]
        [InlineAutoNSubstituteData(default!)]
        [InlineAutoNSubstituteData("")]
        [InlineAutoNSubstituteData(" ")]
        public async Task SearchAsync_ThrowsArgumentException_WhenQueryIsNullOrWhitespace(
            string query,
            [Frozen] ILogger<MovieService> logger,
            [Frozen] ITMDbClient tmdbClient,
            [Frozen] IYouTubeClient youTubeClient)
        {
            // Arrange
            var sut = new MovieService(logger, tmdbClient, youTubeClient);

            // Act
            Func<Task> act = async () => await sut.SearchAsync(query, 1, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Movie query cannot be null or empty*");
        }
    }
}