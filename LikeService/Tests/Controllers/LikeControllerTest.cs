using LikeService.AsyncDataServices;
using LikeService.Clients;
using LikeService.Controllers;
using LikeService.Data;
using LikeService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LikeService.Tests.Controllers;

public class LikeControllerTests
{
    private readonly LikeController _likeController;
    private readonly Mock<IPostServiceClient> _postServiceClientMock;
    private readonly Mock<ILikeRepository> _likeRepositoryMock;
    private readonly Mock<IMessageBusClient> _messageBusClientMock;

    public LikeControllerTests()
    {
        _postServiceClientMock = new Mock<IPostServiceClient>();
        _likeRepositoryMock = new Mock<ILikeRepository>();
        _messageBusClientMock = new Mock<IMessageBusClient>();

        _likeController = new LikeController(
            _messageBusClientMock.Object,
            _likeRepositoryMock.Object,
            _postServiceClientMock.Object
        );
    }

    [Fact]
    public async Task AddLike_ExistingPost_ReturnsOk()
    {
        // Arrange
        int postId = 1;
        _postServiceClientMock.Setup(x => x.CheckPostExistence(postId)).ReturnsAsync(true);

        // Act
        var result = await _likeController.AddLike(postId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(result);
        Assert.Equal("Like added!", (result as OkObjectResult)?.Value);
        _likeRepositoryMock.Verify(x => x.Create(It.IsAny<Like>()), Times.Once);
        _messageBusClientMock.Verify(x => x.AddLike(postId), Times.Once);
    }

    [Fact]
    public async Task AddLike_NonExistingPost_ReturnsNotFound()
    {
        // Arrange
        int postId = 1;
        _postServiceClientMock.Setup(x => x.CheckPostExistence(postId)).ReturnsAsync(false);

        // Act
        var result = await _likeController.AddLike(postId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(result);
        Assert.Equal("Post does not exist", (result as NotFoundObjectResult)?.Value);
        _likeRepositoryMock.Verify(x => x.Create(It.IsAny<Like>()), Times.Never);
        _messageBusClientMock.Verify(x => x.AddLike(postId), Times.Never);
    }

    [Fact]
    public async Task AddLike_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        int postId = 1;
        _postServiceClientMock.Setup(x => x.CheckPostExistence(postId)).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _likeController.AddLike(postId);

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, (result as ObjectResult)!.StatusCode);
        Assert.Equal("Could not send message", (result as ObjectResult)!.Value);
        _likeRepositoryMock.Verify(x => x.Create(It.IsAny<Like>()), Times.Never);
        _messageBusClientMock.Verify(x => x.AddLike(postId), Times.Never);
    }
}
