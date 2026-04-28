using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class CustomerEditViewModel
    {
        public int CId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Adress { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\+?[0-9\s\-]{7,15}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNr { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;
    }
}