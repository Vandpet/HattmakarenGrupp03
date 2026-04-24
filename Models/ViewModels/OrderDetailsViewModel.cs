using System.Collections;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Order? Order { get; set; }
        public List<HatOrder> HatOrders { get; set; } = new();
        public decimal PriceWithoutVat { get; set; }
        public bool IsForeignCustomer { get; set; } //Om kunden är från annat land.
        public List<Material> Materials { get; set; } = new();
    }
}
