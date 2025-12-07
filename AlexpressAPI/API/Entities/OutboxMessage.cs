using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class OutboxMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsProcessed { get; set; }

        [Required] 
        [Column(TypeName = "nvarchar(30)")]
        public string Type { get; set; }

        [Required]
        public string Payload { get; set; }

        public DateTimeOffset? ProcessedTime { get; set; }
    }
}
