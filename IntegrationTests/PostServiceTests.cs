using System.Net.Http.Headers;
using System.Net.Http.Json;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests;

[Collection("PostServiceTests")]
public class PostServiceTests
{
    private readonly PostServiceTestFixture _fixture;
    private readonly Auth0Helper _auth0Helper;

    public PostServiceTests(PostServiceTestFixture fixture)
    {
        _fixture = fixture;
        _auth0Helper = _fixture.Auth0Helper;
    }

    [Fact]
    public async Task Can_Call_Get_Posts_Endpoint()
    {
        // Arrange
        var postgresContainer = await _fixture.StartPostgresContainerAsync();
        var rabbitmqContainer = await _fixture.StartRabbitMQContainerAsync();
        var postServiceContainer = await _fixture.StartPostServiceContainerAsync(postgresContainer, rabbitmqContainer);

        var postsServicePort = postServiceContainer.GetMappedPublicPort(8080);

        var httpClient = new HttpClient();
        var requestUri = new UriBuilder(
            Uri.UriSchemeHttp,
            "localhost",
            postsServicePort,
            "api/posts"
        ).Uri;

        // Act
        var response = await httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("[]", content);
    }

    [Fact]
    public async Task Can_Post()
    {
        // Arrange
        var postgresContainer = await _fixture.StartPostgresContainerAsync();
        var rabbitmqContainer = await _fixture.StartRabbitMQContainerAsync();
        var postServiceContainer = await _fixture.StartPostServiceContainerAsync(postgresContainer, rabbitmqContainer);

        var postsServicePort = postServiceContainer.GetMappedPublicPort(8080);
        var httpClient = new HttpClient();
        var requestUri = new UriBuilder(
            Uri.UriSchemeHttp,
            "localhost",
            postsServicePort,
            "api/posts"
        ).Uri;

        var post = new { content = "Post integration test", author = "Test person" };

        var token = await _auth0Helper.GetAuth0TokenAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await httpClient.PostAsJsonAsync(requestUri, post);
        response.EnsureSuccessStatusCode();

        // Assert
        var getResponse = await httpClient.GetStringAsync(requestUri);
        Assert.Contains("Post integration test", getResponse);
    }

    [Fact]
    public async Task Can_Get_Specific_Post()
    {
        var postgresContainer = await _fixture.StartPostgresContainerAsync();
        var rabbitmqContainer = await _fixture.StartRabbitMQContainerAsync();
        var postServiceContainer = await _fixture.StartPostServiceContainerAsync(postgresContainer, rabbitmqContainer);

        var httpClient = new HttpClient();
        var postsServicePort = postServiceContainer.GetMappedPublicPort(8080);

        var createPostUri = new UriBuilder(
            Uri.UriSchemeHttp,
            "localhost",
            postsServicePort,
            "api/posts"
        ).Uri;

        var token = await _auth0Helper.GetAuth0TokenAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var testPost = new { content = "Post integration test", author = "Test person" };
        var createResponse = await httpClient.PostAsJsonAsync(createPostUri, testPost);
        createResponse.EnsureSuccessStatusCode();

        var getResponse = await httpClient.GetStringAsync(createPostUri);
        Assert.Contains("Post integration test", getResponse);

        var getSpecificUri = new UriBuilder(
            Uri.UriSchemeHttp,
            "localhost",
            postsServicePort,
            "api/posts/1"
        ).Uri;
        var getSpecificResponse = await httpClient.GetStringAsync(getSpecificUri);

        Assert.Contains("Post integration test", getSpecificResponse);
    }
}
