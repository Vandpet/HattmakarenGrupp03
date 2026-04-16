using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class CustomActivityCreateViewModel
    {
        [Required(ErrorMessage = "Namn måste fyllas i")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum måste väljas")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Du måste välja en anställd")]
        public int EId { get; set; }

        public List<SelectListItem> Employees { get; set; } = new();
    }
}