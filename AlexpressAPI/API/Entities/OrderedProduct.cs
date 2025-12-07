using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class OrderedProduct
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; } 
    }
}
