using AutoFixture.Xunit2;
using BookChoice.MediaService.Business.Services.Movies;
using BookChoice.MediaService.Controllers;
using BookChoice.MediaService.Data.Models.TMDb;
using BookChoice.MediaService.Helpers;
using BookChoice.MediaService.Tests.Attributes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BookChoice.MediaService.Tests.Controllers
{
    public class MoviesControllerTests
    {
        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ReturnsNotFound_WhenMovieIsNull(
            string id,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            bool includeYouTubeVideos = true;
            int maxYouTubeResults = 10;
            movieService.GetAsync(id, includeYouTubeVideos, maxYouTubeResults, default).Returns((Movie?)null);
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(id, includeYouTubeVideos, maxYouTubeResults);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ReturnsOk_WhenMovieIsFound(
            string id,
            Movie movie,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            bool includeYouTubeVideos = true;
            int maxYouTubeResults = 10;
            movieService.GetAsync(id, includeYouTubeVideos, maxYouTubeResults, default).Returns(movie);
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(id, includeYouTubeVideos, maxYouTubeResults);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(movie);
            await movieService.Received(1).GetAsync(id, includeYouTubeVideos, maxYouTubeResults, Arg.Any<CancellationToken>());
            cache.Received(1).CreateEntry(CacheKeyHelper.CreateMovieKey(id, includeYouTubeVideos, maxYouTubeResults));
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionIsThrown(
            string id,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            bool includeYouTubeVideos = true;
            int maxYouTubeResults = 10;
            movieService.GetAsync(id, includeYouTubeVideos, maxYouTubeResults, default)
                .Returns<Task<Movie?>>(_ => throw new Exception("Test exception"));
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(id, includeYouTubeVideos, maxYouTubeResults);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            (result as ObjectResult)!.Value.Should().Be("An error occurred while processing your request.");
        }

        [Theory, AutoNSubstituteData]
        public async Task GetAsync_ReturnsCachedMovie_WhenCacheHit(
            string id,
            Movie movie,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            bool includeYouTubeVideos = true;
            int maxYouTubeResults = 10;
            cache.TryGetValue(CacheKeyHelper.CreateMovieKey(id, includeYouTubeVideos, maxYouTubeResults), out Arg.Any<Movie?>()).Returns(x =>
            {
                x[1] = movie;
                return true;
            });
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(id, includeYouTubeVideos, maxYouTubeResults);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(movie);
            await movieService.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ReturnsOk_WhenResultsFound(
            string query,
            int page,
            MovieSearchResults searchResults,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            movieService.SearchAsync(query, page, Arg.Any<CancellationToken>()).Returns(searchResults);
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.SearchAsync(query, page);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(searchResults);
            await movieService.Received(1).SearchAsync(query, page, Arg.Any<CancellationToken>());
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ReturnsInternalServerError_WhenExceptionIsThrown(
            string query,
            int page,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            movieService.SearchAsync(query, page, Arg.Any<CancellationToken>()).Returns<Task<MovieSearchResults?>>(_ => throw new Exception("Test exception"));
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.SearchAsync(query, page);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            (result as ObjectResult)!.Value.Should().Be("An error occurred while processing your request.");
        }

        [Theory, AutoNSubstituteData]
        public async Task SearchAsync_ReturnsCachedResults_WhenCacheHit(
            string query,
            int page,
            MovieSearchResults cachedSearchResult,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            cache.TryGetValue(CacheKeyHelper.CreateSearchKey(query, page), out Arg.Any<IEnumerable<Movie>>()!).Returns(x =>
            {
                x[1] = cachedSearchResult;
                return true;
            });
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.SearchAsync(query, page);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(cachedSearchResult);
            await movieService.DidNotReceive().SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }
    }
}