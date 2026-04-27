using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class CustomActivity
    {
        [Key]
        public int AId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public int EId { get; set; } // Foreign key till Employee
        public Employee? Employee { get; set; }

        public TimeSpan? Time { get; set; }
    }
}