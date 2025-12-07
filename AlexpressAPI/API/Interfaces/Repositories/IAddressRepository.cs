using API.DTOs.AddressesDTO;
using API.Entities;

namespace API.Interfaces.Repositories
{
    public interface IAddressRepository
    {
        Task<IReadOnlyList<AddressDto>> GetAddressesAsync(string userId);
        Task<Address?> GetAddressAsync(int id);
        Task AddAddressAsync(Address a);
        Task SaveContextChangesAsync();
        Task<bool> AddressExistsAsync(int addressId, string userId);
    }
}
