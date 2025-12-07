using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class ReviewItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = "tinyint")] public int Rating { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsDisabled { get; set; }
        [Column(TypeName = "nvarchar(150)")] public string? Comment { get; set; }
        [Column(TypeName = "nvarchar(20)")] public string Author { get; set; }
    }
}
