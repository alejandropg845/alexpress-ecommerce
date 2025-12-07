namespace API.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task<bool> UserExistsAsync(string id);
        Task<bool> UserExistsByEmailAsync(string email);
        Task<bool> UserExistsByNameAsync(string username);
        Task<Dictionary<string, string>> GetUsernamesForProductsAsync(IEnumerable<string> usersIds);
        Task<string?> GetUsernameForProductAsync(string userId);
    }
}
