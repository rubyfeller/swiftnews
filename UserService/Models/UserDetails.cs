using Newtonsoft.Json;

namespace UserService.Models;
public class UserDetails
{
    [JsonProperty("sub")]
    public required string Id { get; set; }

    [JsonProperty("name")]
    public required string Username { get; set; }
}
