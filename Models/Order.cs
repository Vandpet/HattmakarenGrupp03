using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Order
    {
        [Key]
        public int OId { get; set; } // Markeras som OrderId*
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
        public virtual Customer? Customer { get; set; }

        // Den som skapade ordern
        public int CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public virtual Employee? CreatedBy { get; set; }

        // Relationer
        public virtual ICollection<Hat>? Hats { get; set; }

        public List<AssignedOrders> AssignedEmployees { get; set; } = new(); // Förhindrar att man hämtar null
        public List<OrderOfMaterials> MaterialOrders { get; set; } = new(); // Förhindrar att man hämtar null

        public List<HatOrder> HatOrders { get; set; } = new(); // Förhindrar att man hämtar null
    }
}
