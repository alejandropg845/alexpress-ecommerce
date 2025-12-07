using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class CartProduct
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = "varchar(300)")] public string Image { get; set; }
        [Column(TypeName = "varchar(150)")] public string Title { get; set; }
        public int Quantity { get; set; }
        public string? CouponName { get; set; }
        public int CustomizedDiscount { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal NewPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ShippingPrice { get; set; }


    }
}
