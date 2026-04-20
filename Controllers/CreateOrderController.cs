using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HatOrderRepository _hatOrderRepo;
        private readonly OrderRepository _orderRepo;

        public OrderController(ApplicationDbContext context, HatOrderRepository hatOrderRepo, OrderRepository orderRepo)
        {
            _context = context;
            _hatOrderRepo = hatOrderRepo;
            _orderRepo = orderRepo;
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
                .Include(o => o.HatOrders)
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

            //Defaultfilter
            if (string.IsNullOrEmpty(filter)) filter = "alla-ordrar";
            ViewBag.Filter = filter;

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
                    .ToListAsync(),
                PrelDeliveryDate = DateTime.Now.AddDays(14)
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
            if (currentEmployeeId == null) return RedirectToAction("Login", "Auth");

            //Denna kollar för tillfället bara på standardhattar, inte specialhattar.
            if (model.HatRows.Amount.Any(a => a > 0) == false) ModelState.AddModelError(string.Empty, "Du måste välja minst en hatt.");

            if (ModelState.IsValid)
            {
                //Skapar Ordern
                var newOrder = new Order
                {
                    CustomerId = (int)model.SelectedCustomerId!, //Om vi kommer in här så är den inte null.
                    CreatedById = currentEmployeeId.Value, // Här mappar vi inloggad användare!
                    //Price = totalPrice, Denna sätts senare här under!
                    Status = "Ej Påbörjad",
                    Express = model.IsExpress,
                    Discount = model.Discount,
                    DiscountDesc = model.DiscountDesc ?? "Ingen beskrivning tillgänglig",
                    OrderDate = DateTime.Now,
                    PrelDeliveryDate = model.PrelDeliveryDate,
                    Description = model.Description ?? "Ingen beskrivning tillgänglig"
                };

                //Ordern måste skapas, då får vi ett OId att leka med.
                _context.Add(newOrder);
                await _context.SaveChangesAsync();

                //Skapar HatOrder-posterna
                var hatOrders = new List<HatOrder>();

                //Loopar genom vilka standardhattar som valts.
                for (int i = 0; i < model.HatRows.HId.Count; i++)
                {
                    //Om amount är större än 0, vill man ha hatten
                    if (model.HatRows.Amount[i] > 0)
                    {
                        var hatOrder = new HatOrder
                        {
                            HId = model.HatRows.HId[i],
                            OId = newOrder.OId,
                            Amount = model.HatRows.Amount[i]
                        };
                        hatOrders.Add(hatOrder);
                    }
                }

                //Loop för att spara specialhattar här.

                await _hatOrderRepo.AddManyAsync(hatOrders);
                await _hatOrderRepo.SetPriceOnOrder(newOrder.OId);
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

        //Get: Order/Details
        public async Task<IActionResult> Details(int oId)
        {
            // 1. Hämta EmployeeId från Sessionen
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            // 2. Säkerhetskoll: Om sessionen gått ut eller man inte är inloggad
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var currentOrder = await _orderRepo.GetOrderByIdWithCustomerAndCreatorAsync(oId);
            if (currentOrder == null) return NotFound();

            OrderDetailsViewModel viewModel = new OrderDetailsViewModel
            {
                order = currentOrder,
                hatOrders = await _hatOrderRepo.GetByOrderIdAsync(oId)
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SendOrder(int oId)
        {
            // 1. Hämta EmployeeId från Sessionen
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            // 2. Säkerhetskoll: Om sessionen gått ut eller man inte är inloggad
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var orderToSend = await _orderRepo.GetByIdAsync(oId);
            if (orderToSend == null) return NotFound();
            
            orderToSend.Status = "Skickad";

            var hatOrderList = await _hatOrderRepo.GetByOrderIdAsync(oId);
            foreach (var hatOrder in hatOrderList)
            {
                hatOrder.Status = "Skickad";
                await _hatOrderRepo.UpdateAsync(hatOrder);
            }
            
            await _orderRepo.UpdateAsync(orderToSend);
            return RedirectToAction(nameof(Details), new { oId });
        }
    }
}