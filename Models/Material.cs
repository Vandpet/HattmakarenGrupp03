using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Material
    {
        [Key]
        public int MId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string MeasuringUnits { get; set; }
        public decimal Price { get; set; }

        // Navigation till kopplingstabellen
        public ICollection<HatMaterial> MaterialsForHats { get; set; } = new List<HatMaterial>();
        public ICollection<MaterialOrder> MaterialOrders { get; set; } = new List<MaterialOrder>();
    }
}