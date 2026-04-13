using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Material
    {
        [Key]
        public int MId { get; set; } // Markeras som MID*
        public required string Name { get; set; }
        public required decimal Amount { get; set; }
        public required string MeasuringUnits { get; set; }
        public required decimal Price { get; set; }
        // Relationer
        public required virtual ICollection<Hat> Hats { get; set; }
        public required virtual ICollection<MaterialOrder> MaterialOrders { get; set; }

        public List<HatMaterial> MaterialsForHats { get; set; } = new(); // Förhindrar att man hämtar null
    }
}
