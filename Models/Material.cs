using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Material
    {
        [Key]
        public int MId { get; set; }
        public required string Name { get; set; }
        public required double Amount { get; set; }
        public required string MeasuringUnits { get; set; }
        public required decimal Price { get; set; }

        // Navigation till kopplingstabellen
        public List<HatMaterial> MaterialsForHats { get; set; } = new();
        public List<MaterialOrder> MaterialOrders { get; set; } = new();
    }
}