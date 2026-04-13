namespace HattmakarenWebbAppGrupp03.Models
{
    public class OrderOfMaterials
    {
        public int OId { get; set; }
        public Order Order { get; set; }
        public int MoId { get; set; }
        public MaterialOrder MaterialOrder { get; set; }
    }
}
