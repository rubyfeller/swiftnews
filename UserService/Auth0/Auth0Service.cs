using System.Net.Http.Headers;
using Newtonsoft.Json;
using UserService.Models;

namespace UserService.Auth0;

public class Auth0Service : IAuth0Service
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public Auth0Service(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<UserDetails> GetUserDetails(string jwtToken)
    {
        var baseUrl = _configuration["Auth0:BaseUrl"];
        var client = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/userinfo");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<UserDetails>(content);
    }
}
