using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Employee
    {
        [Key] public int EId { get; set; }
        public required string Name { get; set; }
        public required string Adress { get; set; }
        public int PhoneNr { get; set; }
        public int accesslevel { get; set; }

        // Relationer
        public required virtual ICollection<Hat> CreatedHats { get; set; }
        public required virtual ICollection<MaterialOrder> CreatedMaterialOrders { get; set; }
        public required virtual ICollection<Order> HandledOrders { get; set; }
        public List<CustomerManager> ManagedCustomers { get; set; } = new();

    }
}
