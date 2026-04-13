using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Customer
    {
        [Key]
        public int CId { get; set; } // Markeras som Cid* i diagrammet

        [Required]
        public string Name { get; set; }

        [Required]
        public string Adress { get; set; }

        [Required]
        public  string PhoneNr { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Language { get; set; }

        // Relationer
        public  virtual ICollection<Order> Orders { get; set; }
        public List<CustomerManager> Managed { get; set; } = new();
    }
}
