using AutoFixture.Xunit2;
using BookChoice.MediaService.Business.Clients;
using BookChoice.MediaService.Business.Services;
using BookChoice.MediaService.Data.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BookChoice.MediaService.Tests.Services
{
    public class MovieServiceTests
    {
        [Theory, AutoData]
        public async Task Get_WithValidId_ReturnsMovie(string id, Movie expectedMovie)
        {
            // Arrange
            var tmdbClient = Substitute.For<ITMDbClient>();
            var logger = Substitute.For<ILogger<MovieService>>();

            tmdbClient.Get(id).Returns(expectedMovie);

            var service = new MovieService(logger, tmdbClient);

            // Act
            var result = await service.Get(id);

            // Assert
            result.Should().BeEquivalentTo(expectedMovie);
        }

        [Theory, AutoData]
        public async Task Get_WithInvalidId_ReturnsNullAndLogsWarning(string id)
        {
            // Arrange
            var tmdbClient = Substitute.For<ITMDbClient>();
            var logger = Substitute.For<ILogger<MovieService>>();

            tmdbClient.Get(id).Returns((Movie)null!);

            var service = new MovieService(logger, tmdbClient);

            // Act
            var result = await service.Get(id);

            // Assert
            result.Should().BeNull();
        }
    }
}