using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Controllers
{
	public class OrderController : Controller
	{
		private readonly ApplicationDbContext _context;

		public OrderController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Order/Create
		public async Task<IActionResult> Create()
		{
			var viewModel = new CreateOrderViewModel
			{
				// Hämta bara lagerförda hattar
				StandardHats = await _context.Hats.Where(h => h.StandardHat).ToListAsync(),
				// Förbered kundlistan för en dropdown
				CustomerList = await _context.Customers
					.Select(c => new SelectListItem { Value = c.CId.ToString(), Text = c.Name })
					.ToListAsync()
			};
			return View(viewModel);
		}

		// POST: Order/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateOrderViewModel model)
		{
			if (ModelState.IsValid)
			{
				// 1. Hämta de faktiska hattarna från DB för att få priset
				var selectedHats = await _context.Hats
					.Where(h => model.SelectedHatIds.Contains(h.HId))
					.ToListAsync();

				// 2. Beräkna totalpris
				decimal totalPrice = selectedHats.Sum(h => h.Price);
				if (model.IsExpress) totalPrice *= 1.2m; // 20% pålägg för express

				// 3. Skapa Order-objektet
				var newOrder = new Order
				{
					CustomerId = model.SelectedCustomerId,
					Price = totalPrice,
					Status = "Beställning inkommit",
					Express = model.IsExpress,
					Discount = 0,
					DiscountDesc = "Ingen",
					OrderDate = DateTime.Now,
					PrelDeliveryDate = model.PrelDeliveryDate,
					Description = model.Description,
					Hats = selectedHats, // Kopplar hattarna till ordern
					Customer = await _context.Customers.FindAsync(model.SelectedCustomerId) // Krävs pga 'required'
				};

				_context.Add(newOrder);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index", "Home"); // Eller till en Order-lista
			}

			// Om något gick fel, ladda om listorna
			model.StandardHats = await _context.Hats.Where(h => h.StandardHat).ToListAsync();
			model.CustomerList = await _context.Customers
				.Select(c => new SelectListItem { Value = c.CId.ToString(), Text = c.Name })
				.ToListAsync();
			return View(model);
		}
	}
}