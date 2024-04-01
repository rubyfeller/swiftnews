namespace LikeService.Clients;

public class PostServiceClient : IPostServiceClient
{
    private readonly HttpClient _client;

    public PostServiceClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<bool> CheckPostExistence(int postId)
    {
        try
        {
            var response = await _client.GetAsync($"posts/{postId}/exists");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return bool.Parse(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not check post existence: {ex.Message}");
            return false;
        }
    }
}
