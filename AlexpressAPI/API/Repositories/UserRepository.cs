using API.DbContexts;
using API.Entities;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class UserRepository : IUsersRepository
    {
        private readonly AuthDbContext _context;
        public UserRepository(AuthDbContext u)
        {
            _context = u;
        }
        public async Task<bool> UserExistsAsync(string id)
        => await _context.Users.AnyAsync(u => u.Id == id);
        public async Task<bool> UserExistsByNameAsync(string username)
        => await _context.Users.AnyAsync(u => u.UserName == username);
        public async Task<bool> UserExistsByEmailAsync(string email)
        => await _context.Users.AnyAsync(u => u.Email == email);
        public async Task<Dictionary<string, string>> GetUsernamesForProductsAsync(IEnumerable<string> usersIds)
        {
            var usernames = await _context.Users
                .Where(u => usersIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName!);

            return usernames;
        }
        public async Task<string?> GetUsernameForProductAsync(string userId)
        {
            string? username = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.UserName).FirstOrDefaultAsync();

            return username;
        }
    }
}
