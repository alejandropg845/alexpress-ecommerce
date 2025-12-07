
namespace API.DTOs.AddressesDTO
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public string Residence { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}
