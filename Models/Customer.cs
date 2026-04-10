using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Customer
    {
        [Key]
        public required int CId { get; set; } // Markeras som Cid* i diagrammet
        public required string Name { get; set; }
        public required string Adress { get; set; }
        public required string PhoneNr { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string Language { get; set; }

        // Relationer
        public  required virtual ICollection<Order> Orders { get; set; }
        public List<CustomerManager> Managed { get; set; } = new();
    }
}
