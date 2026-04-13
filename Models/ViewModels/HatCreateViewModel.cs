using HattmakarenWebbAppGrupp03.Models;
using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class HatCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Size { get; set; }

        public string? PicturePath { get; set; }

        // Valfria fält (du styr själv vad som ska fyllas i)
        public string? Status { get; set; }
        public bool StandardHat { get; set; }
    }
}
