using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Order
    {
        [Key]
        public int OId { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public bool Express { get; set; }
        public decimal Discount { get; set; }
        public string DiscountDesc { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PrelDeliveryDate { get; set; }
        public string Description { get; set; }

        // Relation till Customer (BelongsTo)
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Den som skapade ordern
        public int CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public Employee? CreatedBy { get; set; }
        // Relationer
        //public ICollection<Hat>? Hats { get; set; }
        public ICollection<OrderOfMaterials>? MaterialOrders { get; set; }
        public ICollection<HatOrder>? HatOrders { get; set; }
    }
}
