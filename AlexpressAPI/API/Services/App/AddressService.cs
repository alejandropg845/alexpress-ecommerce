using API.DTOs.AddressesDTO;
using API.Entities;
using API.Interfaces.Repositories;
using API.Interfaces.Services;
using API.Mappers;

namespace API.Services.App
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        public AddressService(IAddressRepository a)
        {
            _addressRepository = a;
        }
        public async Task<IReadOnlyList<AddressDto>> GetAddressesAsync(string userId)
        => await _addressRepository.GetAddressesAsync(userId);
        public async Task<AddressDto> AddAddressAsync(CreateAddressDto dto, string userId)
        {
            var newAddress = new Address
            {
                AppUserId = userId,
                City = dto.City,
                Country = dto.Country,
                FullName = dto.FullName,
                Phone = dto.Phone,
                PostalCode = dto.PostalCode,
                Residence = dto.Residence
            };

            await _addressRepository.AddAddressAsync(newAddress);

            return newAddress.ToAddressDto();

        }
        public async Task<AddressDto?> UpdateAddressAsync(int addressId, UpdateAddressDto dto)
        {
            var address = await _addressRepository.GetAddressAsync(addressId);

            if (address is null) return null;
            
            address.Phone = dto.Phone;
            address.City = dto.City;
            address.PostalCode = dto.PostalCode;
            address.Country = dto.Country;
            address.FullName = dto.FullName;
            address.Residence = dto.Residence;

            await _addressRepository.SaveContextChangesAsync();

            return address.ToAddressDto();
        }
    }
}
