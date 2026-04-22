using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Hat
    {
        [Key]
        public int HId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public bool StandardHat { get; set; }
        public string PicturePath { get; set; }
        public string Description { get; set; }
        public string KN_Number { get; set; }
        public string KN_Description { get; set; }

        // Relationer
        public ICollection<HatMaterial> Materials { get; set; } = new List<HatMaterial>();
        public ICollection<HatOrder> HatInOrders { get; set; } = new List<HatOrder>();
    }
}
