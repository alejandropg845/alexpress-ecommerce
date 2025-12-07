using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AppUserId { get; set; }
        [Column(TypeName = "nvarchar(70)")] public string FullName { get; set; }
        [Column(TypeName = "varchar(10)")] public string Phone { get; set; }
        [Column(TypeName = "varchar(10)")] public string PostalCode { get; set; }
        [Column(TypeName = "nvarchar(30)")] public string Residence {  get; set; }
        [Column(TypeName = "nvarchar(20)")] public string Country { get; set; }
        [Column(TypeName = "nvarchar(20)")] public string City { get; set; }
    }
}
