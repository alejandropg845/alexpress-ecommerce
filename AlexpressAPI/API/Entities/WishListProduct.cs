using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class WishListProduct
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int WishListId { get; set; }
        public WishList WishList { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
