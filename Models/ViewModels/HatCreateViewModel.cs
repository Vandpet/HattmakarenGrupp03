using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class HatCreateViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public int HId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? PicturePath { get; set; }
        public string? Status { get; set; }
        public bool StandardHat { get; set; } = true;
        public List<string>? KN_options { get; set; }
        [Required(ErrorMessage = "Du måste välja ett KN-nummer")]
        public string? SelectedKN { get; set; }

        public string Description { get; set; }

        // användaren fyller i dessa
        public List<MaterialCreateViewModel> Materials { get; set; } = new();
       
    }

}