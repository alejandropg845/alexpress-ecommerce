using System.ComponentModel.DataAnnotations;

namespace API.DTOs.AddressesDTO
{
    public class CreateAddressDto
    {
        [Required]
        [MaxLength(40, ErrorMessage = "Full name can contain a maximum of 40 characters")]
        public string FullName { get; set; }

        [Required]
        [MaxLength(10,ErrorMessage = "Phone must contain 10 digits")]
        [MinLength(10, ErrorMessage = "Phone must contain 10 digits")]
        public string Phone { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Postal code can contain a maximum of 10 characters")]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(60, ErrorMessage = "Residence can contain a maximum of 60 characters")]
        public string Residence { get; set; }

        [Required]
        [MaxLength(15, ErrorMessage = "Full name can contain a maximum of 15 characters")]
        public string Country { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "City can contain a maximum of 25 characters")]
        public string City { get; set; }
    }
}
