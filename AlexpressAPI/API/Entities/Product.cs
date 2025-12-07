using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AppUserId { get; set;}
        [Column(TypeName = "nvarchar(20)")] public string Username { get; set;}
        public List<string> Images { get; set; }
        [Column(TypeName = "nvarchar(160)")] public string Title { get; set;}
        [Column(TypeName = "nvarchar(2000)")] public string Description { get; set;}
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int Votes { get; set; }
        public int Accumulated {  get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }
        public int ConditionId { get; set; }
        public Condition Condition { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ShippingPrice { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set;}
        public Coupon Coupon { get; set; }
        public List<ReviewItem> Reviews { get; set; }
        public bool IsDeleted { get; set; }
        [Timestamp] public byte[] RowVersion { get; set; }

    }
}
