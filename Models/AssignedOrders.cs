namespace HattmakarenWebbAppGrupp03.Models
{
    public class AssignedOrders
    {
        public int EId { get; set; }
        public Employee Employee { get; set; }
        public int OId { get; set; }
        public Order Order { get; set; }
        public string Description { get; set; }
    
    }
}
