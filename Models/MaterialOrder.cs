using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class MaterialOrder
    {
        [Key]
        public required int MoId { get; set; }
        public bool Printed { get; set; }

        // Relationer
        public required virtual ICollection<Material> Materials { get; set; }
        public List<OrderOfMaterials> Orders { get; set; } = new(); // Förhindrar att man hämtar null
    }
}
