using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class MaterialOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaterialOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var materialOrders = await _context.MaterialOrders
                .ToListAsync();

            return View(materialOrders);
        }

        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.HatOrders)
                    .ThenInclude(ho => ho.Hat)
                        .ThenInclude(h => h.Materials)
                            .ThenInclude(hm => hm.Material)
                .Where(o => o.Status != "Material beställt")
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Preview(List<int> selectedOrderIds)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (selectedOrderIds == null || !selectedOrderIds.Any())
            {
                TempData["ErrorMessage"] = "Du måste välja minst en order.";
                return RedirectToAction(nameof(Create));
            }

            var selectedOrders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.HatOrders)
                    .ThenInclude(ho => ho.Hat)
                        .ThenInclude(h => h.Materials)
                            .ThenInclude(hm => hm.Material)
                .Where(o => selectedOrderIds.Contains(o.OId))
                .ToListAsync();

            var materialSummary = selectedOrders
                .SelectMany(o => o.HatOrders ?? new List<HatOrder>())
                .Where(ho => ho.Hat != null && ho.Hat.Materials != null)
                .SelectMany(ho => ho.Hat.Materials.Select(hm => new
                {
                    MaterialId = hm.Material.MId,
                    MaterialName = hm.Material.Name,
                    Quantity = ho.Amount
                }))
                .GroupBy(x => new
                {
                    x.MaterialId,
                    x.MaterialName
                })
                .Select(group => new MaterialSummaryItem
                {
                    MaterialId = group.Key.MaterialId,
                    MaterialName = group.Key.MaterialName,
                    Quantity = group.Sum(x => x.Quantity)
                })
                .ToList();

            ViewBag.SelectedOrders = selectedOrders;

            return View(materialSummary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaterialOrder(List<int> selectedMaterialIds, List<int> selectedOrderIds)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (selectedMaterialIds == null || !selectedMaterialIds.Any())
            {
                TempData["ErrorMessage"] = "Du måste välja minst ett material.";
                return RedirectToAction(nameof(Create));
            }

            var materialOrder = new MaterialOrder
            {
                Printed = false
            };

            _context.MaterialOrders.Add(materialOrder);
            await _context.SaveChangesAsync();

            foreach (var orderId in selectedOrderIds)
            {
                var orderOfMaterial = new OrderOfMaterials
                {
                    OId = orderId,
                    MoId = materialOrder.MoId
                };

                _context.OrderOfMaterials.Add(orderOfMaterial);
            }

            var orders = await _context.Orders
                .Where(o => selectedOrderIds.Contains(o.OId))
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Status = "Material beställt";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsSent(int id)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var materialOrder = await _context.MaterialOrders
                .FirstOrDefaultAsync(m => m.MoId == id);

            if (materialOrder == null)
            {
                return NotFound();
            }

            materialOrder.Printed = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }



    public class MaterialSummaryItem
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}