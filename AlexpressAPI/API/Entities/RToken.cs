using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class RToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")] public string RefreshToken {  get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
        public string AppUserId { get; set; }
        public bool IsRevoked { get; set; }
    }
}
