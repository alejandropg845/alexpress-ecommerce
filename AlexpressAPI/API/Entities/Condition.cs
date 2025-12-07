using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Condition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        [Column(TypeName = "varchar(12)")] public string Name { get; set; }
    }
}
