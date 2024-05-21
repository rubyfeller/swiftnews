using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;
public class UserRepository : IUserRepository
{
    private readonly UserContext _context;

    public UserRepository(UserContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<string> ids)
    {
        return await _context.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

}
