using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegrationTests.Helpers
{
    public class Auth0Helper
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _domain;
        private readonly string _audience;

        public Auth0Helper()
        {
            _clientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID");
            _clientSecret = Environment.GetEnvironmentVariable("AUTH0_CLIENT_SECRET");
            _domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN");
            _audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret) || string.IsNullOrEmpty(_domain) || string.IsNullOrEmpty(_audience))
            {
                throw new InvalidOperationException("Auth0 credentials are not set in environment variables.");
            }
        }

        public async Task<string> GetAuth0TokenAsync()
        {
            using (var client = new HttpClient())
            {
                var requestBody = new Dictionary<string, string>
                {
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "audience", _audience },
                    { "grant_type", "client_credentials" }
                };

                var requestContent = new FormUrlEncodedContent(requestBody);
                var response = await client.PostAsync($"https://{_domain}/oauth/token", requestContent);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<Auth0TokenResponse>(responseContent);

                return tokenResponse.AccessToken;
            }
        }

        private class Auth0TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
        }
    }
}
