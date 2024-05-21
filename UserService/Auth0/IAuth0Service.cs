using UserService.Models;

namespace UserService.Auth0;

public interface IAuth0Service
{
    Task<UserDetails> GetUserDetails(string jwtToken);
}
