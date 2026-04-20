using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class HatSchedule
    {
        public int Id { get; set; }

        public int HatOrderId { get; set; }
        public HatOrder HatOrder { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; } = "Planned";
    }
}
