using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Employee
    {
        [Key] public int EId { get; set; }
        public required string Name { get; set; }
        public required string Adress { get; set; }
        public int PhoneNr { get; set; }

    }
}
