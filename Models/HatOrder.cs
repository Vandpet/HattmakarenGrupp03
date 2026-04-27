using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class HatOrder
    {
        public int HId { get; set; }
        public Hat Hat { get; set; }
        public int OId { get; set; }
        public Order Order { get; set; }

        public int? EId { get; set; } = null;
        public Employee? Employee { get; set; }

        public string Status { get; set; } = "Not Started";
        public DateTime? Date {  get; set; }
        public string Note { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}