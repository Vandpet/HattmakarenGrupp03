using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Order
    {
        [Key]
        public required int OId { get; set; } // Markeras som OrderId*
        public required decimal Price { get; set; }
        public required string Status { get; set; }
        public required bool Express { get; set; }
        public required decimal Discount { get; set; }
        public required string DiscountDesc { get; set; }
        public required DateTime OrderDate { get; set; }
        public required DateTime PrelDeliveryDate { get; set; }
        public required string Description { get; set; }

        // Relation till Customer (BelongsTo)
        public required int CustomerId { get; set; }
        public required virtual Customer Customer { get; set; }

        // Relationer
        public required virtual ICollection<Hat> Hats { get; set; }

        public List<AssignedOrders> AssignedEmployees { get; set; } = new();
    }
}
