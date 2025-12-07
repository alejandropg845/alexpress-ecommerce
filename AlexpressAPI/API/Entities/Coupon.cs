using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Coupon
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = ("nvarchar(20)"))] public string? CouponName { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Discount { get; set; }

    }
}
