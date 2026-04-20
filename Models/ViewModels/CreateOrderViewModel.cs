using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
	public class CreateOrderViewModel
	{
		[Required(ErrorMessage = "Välj en kund")]
		public int? SelectedCustomerId { get; set; }

		public string? Description { get; set; } = "";

		public bool IsExpress { get; set; }

        [Required(ErrorMessage = "Välj ett leveransdatum")]
        [DataType(DataType.Date)]
		public DateTime PrelDeliveryDate { get; set; } = DateTime.Now.AddDays(14);
        [Range(0, 100)]
        public decimal Discount { get; set; }
        [StringLength(50)]
        public string? DiscountDesc { get; set; } = "";


        // Listor för att fylla dropdowns i vyn
        public List<SelectListItem>? CustomerList { get; set; }
		public List<Hat>? StandardHats { get; set; }

		//Denna används för att se hur många och vilka standardhattar som valts.
		public HatRow HatRows { get; set; } = new();
        public class HatRow
		{
			public List<int> Amount { get; set; } = new();
			public List<int> HId { get; set; } = new();
        }

    }
}