using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Data.Repositories;

using HattmakarenWebbAppGrupp03.Migrations;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Runtime.InteropServices;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HatOrderRepository _hatOrderRepo;
        private readonly OrderRepository _orderRepo;
        private readonly CustomerRepository _customerRepo;

        public OrderController(ApplicationDbContext context, HatOrderRepository hatOrderRepo, OrderRepository orderRepo, CustomerRepository customerRepo)
        {
            _context = context;
            _hatOrderRepo = hatOrderRepo;
            _orderRepo = orderRepo;
            _customerRepo = customerRepo;
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
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // 3. Filtrera baserat på vald knapp
            orders = filter switch
            {
                "påbörjade" => orders.Where(o => o.Status == "Påbörjad").ToList(),
                "avslutade" => orders.Where(o => o.Status == "Skickad").ToList(),
                "ej-påbörjade" => orders.Where(o => o.Status == "Ej Påbörjad").ToList(),
                "färdig" => orders.Where(o => o.Status == "Färdig").ToList(),
                "returnerad" => orders.Where(o => o.Status == "Helt Returnerad" || o.Status == "Delvis Returnerad").ToList(),
                "alla-ordrar" => orders.OrderBy(o => o.OrderDate).ToList(),
                _ => orders //"nyligen"
            };

            //Defaultfilter
            if (string.IsNullOrEmpty(filter)) filter = "nyligen";
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
            var hasAnyHat =
                model.HatRows?.Amount != null &&
                model.HatRows.Amount.Any(a => a > 0);

            if (!hasAnyHat)
            {
                ModelState.AddModelError(string.Empty, "Du måste välja minst en hatt.");
            }

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
                    Description = model.Description ?? "Ingen beskrivning tillgänglig",
                    DeliveryFee = model.DeliveryFee
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
                await _hatOrderRepo.SetPriceOnOrderAsync(newOrder.OId);
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
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }


            var currentOrder = await _orderRepo.GetOrderByIdWithCustomerAndCreatorAsync(oId);
            if (currentOrder == null) return NotFound();

            var materials = await _context.HatOrders
                .Where(ho => ho.OId == oId)
                .Include(ho => ho.Hat)
                .ThenInclude(h => h.Materials)
                .ThenInclude(hm => hm.Material)
                .SelectMany(ho => ho.Hat.Materials.Select(hm => hm.Material))
                .Distinct()
                .ToListAsync();

            OrderDetailsViewModel viewModel = new OrderDetailsViewModel
            {
                Order = currentOrder,
                HatOrders = await _hatOrderRepo.GetByOrderIdAsync(oId),
                PriceWithoutVat = await _hatOrderRepo.GetPriceWithoutVatAsync(oId),
                IsForeignCustomer = await _customerRepo.IsForeignCustomerAsync(currentOrder.CustomerId),
                Materials = materials
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SendOrder(int oId)
        {
            // 1. Hämta EmployeeId från Sessionen
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            // 2. Säkerhetskoll: Om sessionen gått ut eller man inte är inloggad
            if (currentEmployeeId == null) return RedirectToAction("Login", "Auth");

            var orderToSend = await _orderRepo.GetByIdAsync(oId);
            if (orderToSend == null) return NotFound();
            
            orderToSend.Status = "Skickad";
            orderToSend.SentDate = DateTime.Now;

            var hatOrderList = await _hatOrderRepo.GetByOrderIdAsync(oId);
            foreach (var hatOrder in hatOrderList)
            {
                hatOrder.Status = "Skickad";
                await _hatOrderRepo.UpdateAsync(hatOrder);
            }
            
            await _orderRepo.UpdateAsync(orderToSend);
            return RedirectToAction(nameof(Details), new { oId });
        }

        // GET: Order/DownloadPdf
        public async Task<IActionResult> DownloadPdf(int oId)
        {
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (currentEmployeeId == null)
                return RedirectToAction("Login", "Auth");

            var order = await _orderRepo.GetOrderByIdWithCustomerAndCreatorAsync(oId);
            if (order == null) return NotFound();

            var hatOrders = await _hatOrderRepo.GetByOrderIdAsync(oId);

            var boldFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new iText.Layout.Document(pdf);

            doc.Add(new Paragraph($"Order #{order.OId}").SetFontSize(20).SetFont(boldFont));
            doc.Add(new Paragraph($"Status: {order.Status}"));
            doc.Add(new Paragraph($"Order Date: {order.OrderDate:yyyy-MM-dd}"));
            doc.Add(new Paragraph($"Delivery Date: {order.PrelDeliveryDate:yyyy-MM-dd}"));
            doc.Add(new Paragraph($"Express: {(order.Express ? "Yes" : "No")}"));
            doc.Add(new Paragraph($"Description: {order.Description}"));
            doc.Add(new Paragraph(" "));

            if (order.Customer != null)
            {
                doc.Add(new Paragraph("Customer").SetFontSize(14).SetFont(boldFont));
                doc.Add(new Paragraph($"Name: {order.Customer.Name}"));
                doc.Add(new Paragraph($"Phone: {order.Customer.PhoneNr}"));
                doc.Add(new Paragraph(" "));
            }

            if (hatOrders != null && hatOrders.Any())
            {
                doc.Add(new Paragraph("Hats").SetFontSize(14).SetFont(boldFont));
                var table = new iText.Layout.Element.Table(3).UseAllAvailableWidth();
                table.AddHeaderCell("Hat");
                table.AddHeaderCell("Amount");
                table.AddHeaderCell("Price");

                foreach (var hatOrder in hatOrders)
                {
                    if (hatOrder.Hat != null)
                    {
                        table.AddCell(hatOrder.Hat.Name);
                        table.AddCell(hatOrder.Amount.ToString());
                        table.AddCell(hatOrder.Hat.Price.ToString("C"));
                    }
                }
                doc.Add(table);
                doc.Add(new Paragraph(" "));
            }

            if (order.Discount > 0)
                doc.Add(new Paragraph($"Discount: {order.Discount:C} — {order.DiscountDesc}"));

            doc.Add(new Paragraph($"Total Price: {order.Price:C}").SetFont(boldFont));

            doc.Close();

            return File(ms.ToArray(), "application/pdf", $"order_{order.OId}.pdf");
        }

        // GET: Order/ShippingLabel
        public async Task<IActionResult> ShippingLabel(int oId)
        {
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (currentEmployeeId == null) return RedirectToAction("Login", "Auth");

            var order = await _orderRepo.GetOrderByIdWithCustomerAndCreatorAsync(oId);
            if (order == null) return NotFound();

            var vm = new ShippingLabelViewModel
            {
                OId = order.OId,
                CustomerName = order.Customer?.Name,
                CustomerAddress = order.Customer?.Adress,       // anpassa till dina fält
                CustomerCity = order.Customer?.City,
                CustomerCountry = order.Customer?.Country,
                CustomerId = order.Customer?.CId.ToString(),
                DateCreated = order.OrderDate,
                HatOrders = await _hatOrderRepo.GetByOrderIdAsync(oId)
                // X-fälten lämnas tomma — fylls i av användaren
            };

            return View(vm);
        }

        // Material PDF downloader
        public async Task<IActionResult> DownloadMaterialsPdf(int oId)
        {
            var order = await _orderRepo.GetOrderByIdWithCustomerAndCreatorAsync(oId);
            if (order == null) return NotFound();

            //Markera som nedladdad och spara
            order.Downloaded = true;
            await _orderRepo.UpdateAsync(order);


            // Fetch hat orders with materials included
            var hatOrders = await _context.HatOrders
                .Include(ho => ho.Hat)
                    .ThenInclude(h => h.Materials)
                        .ThenInclude(hm => hm.Material)
                .Where(ho => ho.OId == oId)
                .ToListAsync();

            // Aggregate materials across all hats in this order
            var materials = hatOrders
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
                .ToList();

            var boldFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new iText.Layout.Document(pdf);

            doc.Add(new Paragraph($"Materials for Order #{order.OId}").SetFontSize(20).SetFont(boldFont));
            doc.Add(new Paragraph($"Order Date: {order.OrderDate:yyyy-MM-dd}"));
            doc.Add(new Paragraph($"Delivery Date: {order.PrelDeliveryDate:yyyy-MM-dd}"));
            doc.Add(new Paragraph(" "));

            if (materials.Any())
            {
                doc.Add(new Paragraph("Required Materials").SetFontSize(14).SetFont(boldFont));

                var table = new iText.Layout.Element.Table(3).UseAllAvailableWidth();
                table.AddHeaderCell("Material");
                table.AddHeaderCell("Total Amount");
                table.AddHeaderCell("Unit");

                foreach (var m in materials)
                {
                    table.AddCell(m.Name);
                    table.AddCell(m.TotalAmount.ToString("0.##"));
                    table.AddCell(m.Unit);
                }

                doc.Add(table);
            }
            else
            {
                doc.Add(new Paragraph("No materials found for this order."));
            }

            doc.Close();
            return File(ms.ToArray(), "application/pdf", $"materials_order_{order.OId}.pdf");
        }
        [HttpPost]
        public async Task<IActionResult> DownloadShippingLabel(ShippingLabelViewModel vm)
        {
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (currentEmployeeId == null) return RedirectToAction("Login", "Auth");

            var hatOrders = await _context.HatOrders
                .Include(ho => ho.Hat)
                .Where(ho => ho.OId == vm.OId)
                .ToListAsync();

            double vatRate = 0.25;
            var boldFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new iText.Layout.Document(pdf);

            // Avsändare
            doc.Add(new Paragraph("Hattmakaren AB").SetFont(boldFont).SetFontSize(16));
            doc.Add(new Paragraph(vm.CompanyAddress));
            doc.Add(new Paragraph(vm.CompanyPostalCode.ToUpper()));
            doc.Add(new Paragraph(vm.CompanyCity.ToUpper()));
            doc.Add(new Paragraph(vm.CompanyCountry.ToUpper()));
            doc.Add(new Paragraph(" "));

            // Mottagare
            doc.Add(new Paragraph("Reciver").SetFont(boldFont).SetFontSize(14));
            doc.Add(new Paragraph($"Customer nr: {vm.CustomerId}"));
            doc.Add(new Paragraph(vm.CustomerName));
            doc.Add(new Paragraph(vm.CustomerAddress.ToUpper()));
            doc.Add(new Paragraph(vm.CustomerCity.ToUpper()).SetFont(boldFont));
            doc.Add(new Paragraph(vm.CustomerCountry.ToUpper()).SetFont(boldFont));
            doc.Add(new Paragraph(" "));

            // Försändelse-info
            doc.Add(new Paragraph($"Date: {vm.DateCreated:yyyy-MM-dd}"));
            doc.Add(new Paragraph($"Contents: {vm.Contents}"));
            doc.Add(new Paragraph($"Weight: {vm.Weight} kg"));
            doc.Add(new Paragraph(" "));

            // Hatt-tabell
            doc.Add(new Paragraph("Contents").SetFont(boldFont).SetFontSize(14));
            var table = new iText.Layout.Element.Table(5).UseAllAvailableWidth();
            table.AddHeaderCell("Hat name");
            table.AddHeaderCell("Amount");
            table.AddHeaderCell("KN-number");
            table.AddHeaderCell("KN-description");
            table.AddHeaderCell("Price");

            double totalExVat = 0;

            foreach (var ho in hatOrders)
            {
                if (ho.Hat == null) continue;


                decimal priceExVat = ho.Hat.Price * ho.Amount;  // redan ex moms
                totalExVat += (double)priceExVat;

                table.AddCell(ho.Hat.Name);
                table.AddCell(ho.Amount.ToString());
                table.AddCell(ho.Hat.KN_Number ?? "-");       // lägg till KnNumber på Hat-modellen om det saknas
                table.AddCell(ho.Hat.KN_Description ?? "-");  // samma
                table.AddCell(priceExVat.ToString("-"));
            }
            doc.Add(table);
            doc.Add(new Paragraph(" "));

            // Prissummering
            double vat = vm.ShipOutsideCountry ? 0 : totalExVat * 0.25;
            double total = totalExVat + vat + vm.DeliveryPrice;

            //doc.Add(new Paragraph($"Delivery fee: {vm.DeliveryPrice:C}"));
            //if (!vm.ShipOutsideCountry)
            //    doc.Add(new Paragraph($"Moms (25%): {vat:C}"));
            //else
            //    doc.Add(new Paragraph("Moms: 0 (leverans utanför Sverige)"));

            doc.Add(new Paragraph($"Subtotal: {totalExVat:C}"));
            doc.Add(new Paragraph($"Delivery fee: {vm.DeliveryPrice:C}"));
            doc.Add(new Paragraph(vm.ShipOutsideCountry
                ? "TAX: 0 (export)"
                : $"TAX (25%): {vat:C}"));

            doc.Add(new Paragraph($"Total: {total:C}").SetFont(boldFont));

            doc.Close();
            return File(ms.ToArray(), "application/pdf", $"fraktsedel_order_{vm.OId}.pdf");
        }
    }


}
