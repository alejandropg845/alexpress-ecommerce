using API.DTOs.AddressesDTO;
using API.Entities;

namespace API.Interfaces.Services
{
    public interface IAddressService
    {
        Task<IReadOnlyList<AddressDto>> GetAddressesAsync(string userId);
        Task<AddressDto> AddAddressAsync(CreateAddressDto dto, string userId);
        Task<AddressDto?> UpdateAddressAsync(int addressId, UpdateAddressDto dto);

    }
}
