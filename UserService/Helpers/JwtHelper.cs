using System.IdentityModel.Tokens.Jwt;

namespace UserService.Helpers;

public static class JwtHelper
{
    public static string GetUserId(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
        var userId = (jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value) ?? throw new InvalidOperationException("User ID not found in JWT token");
        return userId;
    }
}
