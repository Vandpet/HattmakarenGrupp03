using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public int EId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Adress { get; set; } = string.Empty;

        [Required]
        public string PhoneNr { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(1, 10)]
        public int accesslevel { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}