using System.Collections;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public string CustomerName { get; set; }

        public string CreatedByName { get; set; }

        public string? StartedByName { get; set; }

        public bool IsStarted { get; set; }

        public string StatusText { get; set; }

        public decimal TotalPrice { get; set; }

        public bool IsExpress { get; set; }

        public string? Description { get; set; }

        public string? ImagePath { get; set; }

        public List<HatOrder> HatOrders { get; set; } = new();



    }
}
