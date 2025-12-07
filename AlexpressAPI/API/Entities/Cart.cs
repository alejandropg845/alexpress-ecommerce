using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Cart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AppUserId {  get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Summary { get; set; }
        public List<CartProduct> CartProducts { get; set; } = [];
    }
}