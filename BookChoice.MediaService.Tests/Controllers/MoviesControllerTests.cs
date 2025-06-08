using AutoFixture.Xunit2;
using BookChoice.MediaService.Business.Services.Movies;
using BookChoice.MediaService.Controllers;
using BookChoice.MediaService.Data.Models.TMDb;
using BookChoice.MediaService.Tests.Attributes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BookChoice.MediaService.Tests.Controllers
{
    public class MoviesControllerTests
    {
        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnBadRequest_WhenIdIsNull(
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService)
        {
            // Arrange
            var controller = new MoviesController(logger, movieService);

            // Act
            var result = await controller.Get(null!, true, 10);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequest.Value.Should().Be("ID cannot be null or empty.");
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnBadRequest_WhenIdIsEmpty(
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService)
        {
            // Arrange
            var controller = new MoviesController(logger, movieService);

            // Act
            var result = await controller.Get(string.Empty, true, 10);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequest.Value.Should().Be("ID cannot be null or empty.");
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnNotFound_WhenMovieIsNull(
            string id,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService)
        {
            // Arrange
            movieService.GetAsync(id, true, 10).Returns((Movie?)null);
            var controller = new MoviesController(logger, movieService);

            // Act
            var result = await controller.Get(id, true, 10);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnOk_WhenMovieIsFound(
            string id,
            Movie movie,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService)
        {
            // Arrange
            movieService.GetAsync(id, true, 10).Returns(movie);
            var controller = new MoviesController(logger, movieService);

            // Act
            var result = await controller.Get(id, true, 10);

            // Assert
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(movie);
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldReturnInternalServerError_WhenExceptionThrown(
            string id,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService)
        {
            // Arrange
            movieService.GetAsync(id, true, 10).Returns<Task<Movie?>>(_ => throw new Exception("Test exception"));
            var controller = new MoviesController(logger, movieService);

            // Act
            var result = await controller.Get(id, true, 10);

            // Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().Be("An error occurred while processing your request.");
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldPassIncludeAdditionalYouTubeVideosFalse_ToService(
            string id,
            Movie movie,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService)
        {
            // Arrange
            movieService.GetAsync(id, false, 10).Returns(movie);
            var controller = new MoviesController(logger, movieService);

            // Act
            var result = await controller.Get(id, false, 10);

            // Assert
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(movie);
            await movieService.Received(1).GetAsync(id, false, 10);
        }

        [Theory, AutoNSubstituteData]
        public async Task Get_ShouldPassMaxYouTubeResults_ToService(
            string id,
            Movie movie,
            [Frozen] ILogger<MoviesController> logger,
            [Frozen] IMovieService movieService)
        {
            // Arrange
            int maxResults = 5;
            movieService.GetAsync(id, true, maxResults).Returns(movie);
            var controller = new MoviesController(logger, movieService);

            // Act
            var result = await controller.Get(id, true, maxResults);

            // Assert
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(movie);
            await movieService.Received(1).GetAsync(id, true, maxResults);
        }
    }
}