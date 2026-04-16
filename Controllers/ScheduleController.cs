using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(bool personal = false, int? year = null, int? month = null)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId")!.Value;
            DateTime today = DateTime.Today;
            int selectedYear = year ?? today.Year;
            int selectedMonth = month ?? today.Month;

            var firstDay = new DateTime(selectedYear, selectedMonth, 1);
            var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);

            var hatOrdersQuery = _context.HatOrders
                .Include(h => h.Hat)
                .Include(h => h.Employee)
                .Include(h => h.Order)
                    .ThenInclude(o => o.Customer)
                .AsQueryable();

            if (personal)
            {
                hatOrdersQuery = hatOrdersQuery.Where(h => h.EId == currentEmployeeId);
            }

            var hatOrders = await hatOrdersQuery.ToListAsync();

            var model = new MonthScheduleViewModel
            {
                IsPersonal = personal,
                Year = selectedYear,
                Month = selectedMonth
            };

            // Oschemalagda uppgifter till vänster
            var unscheduled = hatOrders
                .Where(h => !h.StartDate.HasValue || !h.EndDate.HasValue)
                .ToList();

            foreach (var ho in unscheduled)
            {
                model.UnscheduledTasks.Add(new UnscheduledTaskViewModel
                {
                    OrderId = ho.OId,
                    HatId = ho.HId,
                    Title = $"Order {ho.OId}",
                    HatName = ho.Hat?.Name ?? "Hattuppgift",
                    Status = ho.Status,
                    ColorClass = GetColorClass(ho.Order, ho.Status)
                });
            }

            DateTime current = startDate;

            for (int week = 0; week < 6; week++)
            {
                var weekRow = new WeekRowViewModel();

                for (int day = 0; day < 7; day++)
                {
                    var dayOrders = hatOrders
                        .Where(h =>
                            h.StartDate.HasValue &&
                            h.EndDate.HasValue &&
                            h.StartDate.Value.Date <= current.Date &&
                            h.EndDate.Value.Date >= current.Date)
                        .ToList();

                    var cell = new CalendarCellViewModel
                    {
                        Date = current,
                        IsCurrentMonth = current.Month == selectedMonth,
                        IsToday = current.Date == today
                    };

                    foreach (var ho in dayOrders)
                    {
                        cell.Events.Add(new CalendarEventViewModel
                        {
                            OrderId = ho.OId,
                            HatId = ho.HId,
                            Title = $"Order {ho.OId}",
                            HatName = ho.Hat?.Name ?? "",
                            Status = ho.Status,
                            ColorClass = GetColorClass(ho.Order, ho.Status)
                        });
                    }

                    weekRow.Days.Add(cell);
                    current = current.AddDays(1);
                }

                model.Weeks.Add(weekRow);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTaskToDate(int orderId, int hatId, DateTime date, bool personal = false, int? year = null, int? month = null)
        {
            var hatOrder = await _context.HatOrders
                .FirstOrDefaultAsync(h => h.OId == orderId && h.HId == hatId);

            if (hatOrder == null)
            {
                return NotFound();
            }

            hatOrder.StartDate = date.Date;
            hatOrder.EndDate = date.Date;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new
            {
                personal,
                year = year ?? date.Year,
                month = month ?? date.Month
            });
        }

        private string GetColorClass(Order? order, string hatOrderStatus)
        {
            if (order == null)
                return "order-default";

            if (order.Status == "Klar" || hatOrderStatus == "Klar")
                return "order-green";

            if (order.PrelDeliveryDate.Date < DateTime.Today)
                return "order-red";

            if (order.PrelDeliveryDate.Date <= DateTime.Today.AddDays(3))
                return "order-yellow";

            return "order-default";
        }
    }
}