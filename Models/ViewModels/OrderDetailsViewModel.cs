namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Order order { get; set; }
        public List<HatOrder> hatOrders { get; set; } = new List<HatOrder>();
    }
}
