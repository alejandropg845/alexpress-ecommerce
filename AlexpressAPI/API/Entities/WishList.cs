using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class WishList
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AppUserId {  get; set; }
        public List<WishListProduct> WishListProducts { get; set; } = [];

    }
}
