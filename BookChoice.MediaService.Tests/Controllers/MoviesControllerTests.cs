using AutoFixture.Xunit2;
using BookChoice.MediaService.Business.Services.Movies;
using BookChoice.MediaService.Controllers;
using BookChoice.MediaService.Data.Models.TMDb;
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
        public async Task Get_ShouldReturnBadRequest_WhenIdIsNull(
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(null!);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequest.Value.Should().Be("ID cannot be null or empty.");
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnBadRequest_WhenIdIsEmpty(
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(string.Empty);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequest.Value.Should().Be("ID cannot be null or empty.");
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnNotFound_WhenMovieIsNull(
            string id,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            bool includeYouTubeVideos = true;
            int maxYouTubeResults = 10;
            movieService.GetAsync(id, includeYouTubeVideos, maxYouTubeResults).Returns((Movie?)null);
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(id, includeYouTubeVideos, maxYouTubeResults);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnOk_WhenMovieIsFound(
            string id,
            Movie movie,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            bool includeYouTubeVideos = true;
            int maxYouTubeResults = 10;
            movieService.GetAsync(id, includeYouTubeVideos, maxYouTubeResults).Returns(movie);
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(id, includeYouTubeVideos, maxYouTubeResults);

            // Assert
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(movie);
            await movieService.Received(1).GetAsync(id, includeYouTubeVideos, maxYouTubeResults);
            cache.Received(1).CreateEntry($"movie:{id}:yt:{includeYouTubeVideos}:max:{maxYouTubeResults}");
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnInternalServerError_WhenExceptionThrown(
            string id,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            bool includeYouTubeVideos = true;
            int maxYouTubeResults = 10;
            movieService.GetAsync(id, includeYouTubeVideos, maxYouTubeResults).Returns<Task<Movie?>>(_ => throw new Exception("Test exception"));
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(id, includeYouTubeVideos, maxYouTubeResults);

            // Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().Be("An error occurred while processing your request.");
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnCachedMovie_WhenCacheHit(
            string id,
            Movie movie,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService,
            [Frozen] IMemoryCache cache)
        {
            // Arrange
            bool includeYouTubeVideos = true;
            int maxYouTubeResults = 10;
            cache.TryGetValue($"movie:{id}:yt:{includeYouTubeVideos}:max:{maxYouTubeResults}", out Arg.Any<Movie?>()).Returns(x =>
            {
                x[1] = movie;
                return true;
            });
            var controller = new MoviesController(logger, movieService, cache);

            // Act
            var result = await controller.GetAsync(id, includeYouTubeVideos, maxYouTubeResults);

            // Assert
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(movie);
            await movieService.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>());
        }
    }
}