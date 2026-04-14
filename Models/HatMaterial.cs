namespace HattmakarenWebbAppGrupp03.Models
{
    public class HatMaterial
    {
        public int HatId { get; set; }
        public Hat Hat { get; set; }

        public int MaterialId { get; set; }
        public Material Material { get; set; }
    }
}