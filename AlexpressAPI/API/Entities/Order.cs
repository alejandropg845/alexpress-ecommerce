using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public string AppUserId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Summary { get; set; }
        [Column(TypeName = "tinyint")] public int Rating { get; set; }
        public List<OrderedProduct> OrderedProducts { get; set; }
        public string StripeSessionId { get; set; }
    }
}
