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

		// GET: Order
		public async Task<IActionResult> Index(string filter)
		{
			// 1. Säkerhetskoll (Session)
			if (HttpContext.Session.GetInt32("EmployeeId") == null)
			{
				return RedirectToAction("Login", "Auth");
			}

			// 2. Hämta alla ordrar och "länka in" Customer och CreatedBy (Employee)
			var orders = await _context.Orders
				.Include(o => o.Customer)
				.Include(o => o.CreatedBy)
				.OrderByDescending(o => o.OrderDate) // Nyast först
				.ToListAsync();

            // 3. Filtrera baserat på vald knapp
            orders = filter switch
            {
                "nyligen" => orders.OrderBy(o => o.OrderDate).ToList(),
                "påbörjade" => orders.Where(o => o.Status == "Påbörjad").ToList(),
                "avslutade" => orders.Where(o => o.Status == "Färdig").ToList(),
                "ej-påbörjade" => orders.Where(o => o.Status == "Ej Påbörjad").ToList(),
                _ => orders
            };

            return View(orders);
        }

		// GET: Order/Create
		public async Task<IActionResult> Create()
		{
			// KOLL: Är användaren inloggad?
			if (HttpContext.Session.GetInt32("EmployeeId") == null)
			{
				// Om inte, skicka dem till inloggningssidan
				return RedirectToAction("Login", "Auth");
			}

			var viewModel = new CreateOrderViewModel
			{
				StandardHats = await _context.Hats.Where(h => h.StandardHat).ToListAsync(),
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
			// 1. Hämta EmployeeId från Sessionen
			int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

			// 2. Säkerhetskoll: Om sessionen gått ut eller man inte är inloggad
			//if (currentEmployeeId == null)
			//{
			//	return RedirectToAction("Login", "Auth");
			//}

			if (ModelState.IsValid)
			{
				var selectedHats = await _context.Hats
					.Where(h => model.SelectedHatIds.Contains(h.HId))
					.ToListAsync();

				decimal totalPrice = selectedHats.Sum(h => h.Price);
				if (model.IsExpress) totalPrice *= 1.2m;

				var newOrder = new Order
				{
					CustomerId = model.SelectedCustomerId,
					CreatedById = currentEmployeeId.Value, // Här mappar vi inloggad användare!
					Price = totalPrice,
					Status = "Ej Påbörjad",
					Express = model.IsExpress,
					Discount = 0,
					DiscountDesc = "Ingen",
					OrderDate = DateTime.Now,
					PrelDeliveryDate = model.PrelDeliveryDate,
					Description = model.Description ?? "",
					Hats = selectedHats
				};

				_context.Add(newOrder);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}

			// Om valideringen misslyckas, ladda om listorna som förut
			model.StandardHats = await _context.Hats.Where(h => h.StandardHat).ToListAsync();
			model.CustomerList = await _context.Customers
				.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
				{
					Value = c.CId.ToString(),
					Text = c.Name
				}).ToListAsync();

			return View(model);
		}
	}
}