using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Customer
    {
        [Key]
        public int CId { get; set; } // Markeras som Cid* i diagrammet
        public string Name { get; set; }
        public string Adress { get; set; }
        public  string PhoneNr { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Language { get; set; }

        // Relationer
        public ICollection<Order> Orders { get; set; }
        public ICollection<CustomerManager> Managed { get; set; }
        //public List<CustomerManager> Managed { get; set; } = new(); //Förhindrar att man hämtar null
    }
}
