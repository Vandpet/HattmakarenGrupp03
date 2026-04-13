using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Hat
    {
        [Key]
        public int HId { get; set; } // Markeras som HatID*
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required string Size { get; set; }
        public required string Status { get; set; }
        public required bool StandardHat { get; set; }
        public required string PicturePath { get; set; }

        // Relationer
        //public required virtual ICollection<Material> Materials { get; set; }

        public List<HatMaterial> Materials { get; set; } = new(); // Förhindrar att man hämtar null

    }
}
