using UserService.Models;

namespace UserService.Data;
public interface IUserRepository
{
    Task<User> GetUserByIdAsync(string id);
    Task AddUserAsync(User user);
    Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<string> ids);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
}