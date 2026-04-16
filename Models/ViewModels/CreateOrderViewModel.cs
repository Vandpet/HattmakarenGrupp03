using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
	public class CreateOrderViewModel
	{
		[Required(ErrorMessage = "Välj en kund")]
		public int SelectedCustomerId { get; set; }

		[Required(ErrorMessage = "Välj minst en hatt")]
		public List<int> SelectedHatIds { get; set; } = new();

		public string Description { get; set; } = "";

		public bool IsExpress { get; set; }

		[DataType(DataType.Date)]
		public DateTime PrelDeliveryDate { get; set; } = DateTime.Now.AddDays(14);

		// Listor för att fylla dropdowns i vyn
		public List<SelectListItem>? CustomerList { get; set; }
		public List<Hat>? StandardHats { get; set; }

		//lista för att HatOrder 
		public List<HatOrder> HatOrder { get; set; }

		public List<HatListItem> HatListItems { get; set; } = new();
        public class HatListItem
		{
			public List<int> Amount { get; set; } = new();
			public List<Hat>? StandardHat { get; set; } = new();
        }

    }
}