using API.DbContexts;
using API.DTOs.AddressesDTO;
using API.Entities;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AddressDbContext _context;
        public AddressRepository(AddressDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<AddressDto>> GetAddressesAsync(string userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.AppUserId == userId)
                .Select(a => new AddressDto
                {
                    City = a.City,
                    Country = a.Country,
                    FullName = a.FullName,
                    Id = a.Id,
                    Phone = a.Phone,
                    PostalCode = a.PostalCode,
                    Residence = a.Residence
                })
                .ToListAsync();

            return addresses; 
        }
        public async Task<Address?> GetAddressAsync(int id)
        {
            return await _context
                .Addresses
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task AddAddressAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }
        public async Task SaveContextChangesAsync() => await _context.SaveChangesAsync();

        public async Task<bool> AddressExistsAsync(int addressId, string userId)
        => await _context.Addresses.AnyAsync(a => a.Id == addressId && a.AppUserId == userId);
    }
}
