using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Order
    {
        [Key]
        public required string OrderId { get; set; } // Markeras som OrderId*
        public required decimal Price { get; set; }
        public required string Status { get; set; }
        public required bool Express { get; set; }
        public required decimal Rabatt { get; set; }
        public required string RabattBeskrivning { get; set; }
        public required DateTime BeställningsDatum { get; set; }
        public required DateTime PreliminärtLeveransDatum { get; set; }
        public required string Anteckning { get; set; }

        // Relation till Customer (BelongsTo)
        public required int CustomerId { get; set; }
        public required virtual Customer Customer { get; set; }

        // Relationer
        public required virtual ICollection<Hat> Hats { get; set; }
    }
}
