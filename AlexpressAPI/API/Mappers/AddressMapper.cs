using API.DTOs.AddressesDTO;
using API.Entities;

namespace API.Mappers
{
    public static class AddressMapper
    {
        public static AddressDto ToAddressDto (this Address address)
        {
            return new AddressDto
            {
                Residence = address.Residence,
                PostalCode = address.PostalCode,
                Phone = address.Phone,
                Id = address.Id,
                FullName = address.FullName,
                City = address.City,
                Country = address.Country,
            };
        } 
    }
}
