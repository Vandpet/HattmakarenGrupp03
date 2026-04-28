using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using iText.Commons.Actions.Contexts;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using Microsoft.AspNetCore.Http.HttpResults;
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadCombinedMaterialsPdf(List<int> selectedOrderIds)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Login", "Auth");

            if (selectedOrderIds == null || !selectedOrderIds.Any())
            {
                TempData["ErrorMessage"] = "Du måste välja minst en order.";
                return RedirectToAction(nameof(Create));
            }

            var hatOrders = await _context.HatOrders
                .Include(ho => ho.Hat)
                    .ThenInclude(h => h.Materials)
                        .ThenInclude(hm => hm.Material)
                .Include(ho => ho.Order)
                .Where(ho => selectedOrderIds.Contains(ho.OId))
                .ToListAsync();

            // Aggregate materials across all selected orders
            var materials = hatOrders
                .Where(ho => ho.Hat?.Materials != null)
                .SelectMany(ho => ho.Hat.Materials.Select(hm => new
                {
                    hm.Material.Name,
                    hm.Material.MeasuringUnits,
                    Amount = hm.Material.Amount * ho.Amount
                }))
                .GroupBy(m => new { m.Name, m.MeasuringUnits })
                .Select(g => new
                {
                    Name = g.Key.Name,
                    Unit = g.Key.MeasuringUnits,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderBy(m => m.Name)
                .ToList();

            var boldFont = iText.Kernel.Font.PdfFontFactory.CreateFont(
                iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new iText.Layout.Document(pdf);

            doc.Add(new Paragraph("Materialsammanställning")
                .SetFontSize(20).SetFont(boldFont));

            doc.Add(new Paragraph($"Datum: {DateTime.Today:yyyy-MM-dd}"));
            doc.Add(new Paragraph($"Inkluderade ordrar: {string.Join(", ", selectedOrderIds)}"));
            doc.Add(new Paragraph(" "));

            if (materials.Any())
            {
                doc.Add(new Paragraph("Totalt material")
                    .SetFontSize(14).SetFont(boldFont));

                var table = new iText.Layout.Element.Table(3).UseAllAvailableWidth();
                table.AddHeaderCell("Material");
                table.AddHeaderCell("Total mängd");
                table.AddHeaderCell("Enhet");

                foreach (var m in materials)
                {
                    table.AddCell(m.Name);
                    table.AddCell(m.TotalAmount.ToString("0.##"));
                    table.AddCell(m.Unit ?? "-");
                }

                doc.Add(table);
            }
            else
            {
                doc.Add(new Paragraph("Inga material hittades för valda ordrar."));
            }

            doc.Close();

            var orderIds = string.Join("-", selectedOrderIds);
            return File(ms.ToArray(), "application/pdf", $"material_bestallning_{orderIds}.pdf");
        }


    }



    public class MaterialSummaryItem
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }



    }