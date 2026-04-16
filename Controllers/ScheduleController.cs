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

        public async Task<IActionResult> Index(bool personal = false, int? year = null, int? month = null, int? day = null)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId")!.Value;
            DateTime today = DateTime.Today;

            int selectedYear = year ?? today.Year;
            int selectedMonth = month ?? today.Month;
            int selectedDay = day ?? today.Day;

            // Skydd så vald dag alltid är giltig för månaden
            int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);
            if (selectedDay > daysInMonth)
            {
                selectedDay = daysInMonth;
            }
            if (selectedDay < 1)
            {
                selectedDay = 1;
            }

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

            var hatOrders = await hatOrdersQuery
                .Where(h => h.StartDate.HasValue && h.EndDate.HasValue)
                .ToListAsync();

            var model = new SchedulePageViewModel
            {
                IsPersonal = personal,
                Year = selectedYear,
                Month = selectedMonth,
                SelectedDay = selectedDay
            };

            // Kalenderdagar för vald månad
            for (int d = 1; d <= daysInMonth; d++)
            {
                var date = new DateTime(selectedYear, selectedMonth, d);

                var dayOrders = hatOrders
                    .Where(h => h.StartDate!.Value.Date <= date.Date &&
                                h.EndDate!.Value.Date >= date.Date)
                    .ToList();

                model.Days.Add(new CalendarDayViewModel
                {
                    Date = date,
                    IsCurrentMonth = true,
                    IsSelected = d == selectedDay,
                    HasItems = dayOrders.Any(),
                    ColorClass = GetDayColor(dayOrders)
                });
            }

            var selectedDate = new DateTime(selectedYear, selectedMonth, selectedDay);

            var selectedDayOrders = hatOrders
                .Where(h => h.StartDate!.Value.Date <= selectedDate.Date &&
                            h.EndDate!.Value.Date >= selectedDate.Date)
                .GroupBy(h => h.OId)
                .ToList();

            foreach (var orderGroup in selectedDayOrders)
            {
                var first = orderGroup.First();
                var order = first.Order;

                model.Orders.Add(new OrderGroupViewModel
                {
                    OrderId = first.OId,
                    Title = $"Order {first.OId}",
                    CustomerName = order?.Customer?.Name ?? "",
                    Status = order?.Status ?? "",
                    ColorClass = GetOrderColor(order),
                    Tasks = orderGroup.Select(h => new TaskItemViewModel
                    {
                        Title = h.Hat?.Name ?? "Hattuppgift",
                        AssignedTo = h.Employee?.Name ?? "Ej tilldelad",
                        Status = h.Status,
                        IsDone = h.Status == "Klar"
                    }).ToList()
                });
            }

            // Personliga aktiviteter bara i personligt schema
            if (personal)
            {
                var activities = await _context.CustomActivities
                    .Include(a => a.Employee)
                    .Where(a =>
                        a.EId == currentEmployeeId &&
                        a.StartDate.Date <= selectedDate.Date &&
                        a.EndDate.Date >= selectedDate.Date)
                    .ToListAsync();

                if (activities.Any())
                {
                    model.Orders.Add(new OrderGroupViewModel
                    {
                        OrderId = 0,
                        Title = "Mina aktiviteter",
                        CustomerName = "",
                        Status = "Planerad",
                        ColorClass = "order-default",
                        Tasks = activities.Select(a => new TaskItemViewModel
                        {
                            Title = a.Name,
                            AssignedTo = a.Employee?.Name ?? "",
                            Status = "Planerad",
                            IsDone = false
                        }).ToList()
                    });
                }
            }

            return View(model);
        }

        private string GetOrderColor(Order? order)
        {
            if (order == null)
                return "order-default";

            if (order.Status == "Klar")
                return "order-green";

            if (order.PrelDeliveryDate.Date < DateTime.Today)
                return "order-red";

            if (order.PrelDeliveryDate.Date <= DateTime.Today.AddDays(3))
                return "order-yellow";

            return "order-default";
        }

        private string GetDayColor(List<HatOrder> hatOrders)
        {
            if (!hatOrders.Any())
                return "";

            if (hatOrders.Any(h => h.Order != null &&
                                   h.Order.Status != "Klar" &&
                                   h.Order.PrelDeliveryDate.Date < DateTime.Today))
            {
                return "calendar-red";
            }

            if (hatOrders.Any(h => h.Order != null &&
                                   h.Order.Status != "Klar" &&
                                   h.Order.PrelDeliveryDate.Date <= DateTime.Today.AddDays(3)))
            {
                return "calendar-yellow";
            }

            return "calendar-dark";
        }
    }
}