using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class HatCreateViewModel
    {
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ]+$",
        ErrorMessage = "Name can only contain letters.")]
        public string Name { get; set; }
        public decimal Price { get; set; }
        [StringLength(50)]
        public string Size { get; set; }
        public int HId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? PicturePath { get; set; }
        public string? Status { get; set; }
        public bool StandardHat { get; set; } = true;
        public List<string>? SizeOptions { get; set; } = new List<string> { "XS - 53cm", "S - 55cm", "M - 57cm", "L - 59cm", "XL - 61cm" };
        public List<string>? KN_options { get; set; }
        [Required(ErrorMessage = "Du måste välja ett KN-nummer")]
        public string? SelectedKN { get; set; }

        public string Description { get; set; }

        // användaren fyller i dessa
        public List<MaterialCreateViewModel> Materials { get; set; } = new();
       
    }

}