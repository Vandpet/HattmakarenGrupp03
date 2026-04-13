using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class AssignedOrders
    {
        [Key]
        public int EId { get; set; }
        public Employee Employee { get; set; }

        public int OId { get; set; }
        public Order Order { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
