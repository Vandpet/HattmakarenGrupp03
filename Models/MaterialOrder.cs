using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class MaterialOrder
    {
        [Key]
        public required string ID { get; set; }
        public bool Utskriven { get; set; }

        // Relationer
        public required virtual ICollection<Material> Materials { get; set; }
    }
}
