using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class Employee
    {
        [Key]
        public int EId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;
        public string PhoneNr { get; set; } = string.Empty;

        [Range(1, 10)]
        public int accesslevel { get; set; }
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        // Relationer
        public ICollection<Hat> CreatedHats { get; set; }
        public ICollection<MaterialOrder> CreatedMaterialOrders { get; set; }
        public ICollection<CustomerManager> ManagedCustomers { get; set; }
        public ICollection<HatOrder> AssignedHats { get; set; }
        public ICollection<Order> HandledOrders { get; set; }
        public ICollection<CustomActivity> Activities { get; set; }

        //public List<CustomerManager> ManagedCustomers { get; set; } = new(); // Förhindrar att man hämtar null
        //public List<HatOrder> AssignedHats { get; set; } = new(); // Förhindrar att man hämtar null
        //public List<AssignedOrders> TakenOrders { get; set; } = new(); // Förhindrar att man hämtar null
    }
}
