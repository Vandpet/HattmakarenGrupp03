using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class MaterialOrder
    {
        [Key]
        public int MoId { get; set; }
        public bool Printed { get; set; }

        // Relationer
        public ICollection<Material> Materials { get; set; }
        public ICollection<OrderOfMaterials> Orders { get; set; }
    }
}
