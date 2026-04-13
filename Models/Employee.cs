using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class Employee
    {
        [Key]
        public int EId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Adress { get; set; } = string.Empty;

        [Required]
        public string PhoneNr { get; set; } = string.Empty;

        [Range(1, 10)]
        public int accesslevel { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        // Relationer
        public virtual ICollection<Hat> CreatedHats { get; set; }
        public virtual ICollection<MaterialOrder> CreatedMaterialOrders { get; set; }
        public virtual ICollection<Order> HandledOrders { get; set; }

        public List<AssignedOrders> TakenOrders { get; set; } = new(); // Förhindrar att man hämtar null
        public List<CustomerManager> ManagedCustomers { get; set; } = new();

    }
}
