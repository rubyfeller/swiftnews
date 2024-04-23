using System.Net.Http.Json;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests;

[Collection("LikeServiceTests")]
public class LikeServiceTests
{
    private readonly LikeServiceTestFixture _likeFixture;

    public LikeServiceTests(LikeServiceTestFixture likeFixture)
    {
        _likeFixture = likeFixture;
    }

    [Fact]
    public async Task Can_Call_Get_Likes_Endpoint()
    {
        // Arrange
        var mongoContainer = await _likeFixture.StartMongoContainerAsync();
        var rabbitmqContainer = await _likeFixture.StartRabbitMQContainerAsync();
        var likeServiceContainer = await _likeFixture.StartLikeServiceContainerAsync(mongoContainer, rabbitmqContainer);


        var httpClient = new HttpClient();
        var requestUri =
            new UriBuilder(
                Uri.UriSchemeHttp,
                likeServiceContainer.Hostname,
                likeServiceContainer.GetMappedPublicPort(8081),
                "api/l/likes"
            ).Uri;
        
        // Act
        var like = await httpClient.GetStringAsync(requestUri);
        
        // Assert
        Assert.Equal("[]", like);
    }

    [Fact]
    public async Task Can_Like_Post()
    {
        var postgresContainer = await _likeFixture.StartPostgresContainerAsync();
        var rabbitmqContainer = await _likeFixture.StartRabbitMQContainerAsync();
        var postServiceContainer =
            await _likeFixture.StartPostServiceContainerAsync(postgresContainer, rabbitmqContainer);

        var mongoContainer = await _likeFixture.StartMongoContainerAsync();
        var likeServiceContainer = await _likeFixture.StartLikeServiceContainerAsync(mongoContainer, rabbitmqContainer);

        var httpClient = new HttpClient();
        var requestUri =
            new UriBuilder(
                Uri.UriSchemeHttp,
                postServiceContainer.Hostname,
                postServiceContainer.GetMappedPublicPort(8080),
                "api/posts"
            ).Uri;
        var post = new { content = "Post integration test", author = "Test person" };

        var response = await httpClient.PostAsJsonAsync(requestUri, post);
        response.EnsureSuccessStatusCode();

        var getResponse = await httpClient.GetStringAsync(requestUri);
        Assert.Contains("Post integration test", getResponse);

        var likeUri =
            new UriBuilder(
                Uri.UriSchemeHttp,
                likeServiceContainer.Hostname,
                likeServiceContainer.GetMappedPublicPort(8081),
                "api/l/likes/1"
            ).Uri;
        var likeResponse = await httpClient.PostAsync(likeUri, null);
        likeResponse.EnsureSuccessStatusCode();

        await Task.Delay(1000);
        
        var getUri =
            new UriBuilder(
                Uri.UriSchemeHttp,
                postServiceContainer.Hostname,
                postServiceContainer.GetMappedPublicPort(8080),
                "api/posts/1"
            ).Uri;
        
        var postResponse = await httpClient.GetStringAsync(getUri);
        var postResponseObject = JsonConvert.DeserializeObject<PostResponse>(postResponse);
        if (postResponseObject != null)
        {
            Assert.Equal(1, postResponseObject.LikeCount);
        }
        else
        {
            Assert.Fail("Failed to parse the response into a PostResponse object.");
        }
    }
}