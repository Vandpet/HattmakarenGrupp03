using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HatOrderRepository _hatOrderRepository;

        public ScheduleController(ApplicationDbContext context, HatOrderRepository hatOrderRepository)
        {
            _context = context;
            _hatOrderRepository = hatOrderRepository;
        }

        public async Task<IActionResult> Index(bool personal = false, int? year = null, int? month = null)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Login", "Auth");

            int currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId")!.Value;

            DateTime today = DateTime.Today;
            int selectedYear = year ?? today.Year;
            int selectedMonth = month ?? today.Month;

            var firstDay = new DateTime(selectedYear, selectedMonth, 1);
            var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);


            var hatOrdersQuery = _context.HatOrders
                .Include(h => h.Hat)
                .Include(h => h.Order)
                .Include(h => h.Employee)
                .AsQueryable();

            if (personal)
            {
                hatOrdersQuery = hatOrdersQuery
                    .Where(h => h.EId == currentEmployeeId);
            }

            var hatOrders = await hatOrdersQuery.ToListAsync();


            // HÄMTA ALLA HATORDERS (för oschemalagda)
            var allHatOrders = await _context.HatOrders
                .Include(h => h.Hat)
                .Include(h => h.Order)
                .ToListAsync();

            //var scheduledHatOrderIds = schedules.Select(s => s.HatOrderId).ToHashSet();

            var model = new MonthScheduleViewModel
            {
                IsPersonal = personal,
                Year = selectedYear,
                Month = selectedMonth
            };

            // ✅ OSCHEMALAGDA
            //var unscheduled = allHatOrders
            //    .Where(h => !scheduledHatOrderIds.Contains(h.Id))
            //    .ToList();

            var unscheduled = allHatOrders.Where(ho => ho.Status == "Ej Påbörjad").ToList();
            

            foreach (var ho in unscheduled)
            {
                model.UnscheduledTasks.Add(new UnscheduledTaskViewModel
                {
                    OrderId = ho.OId,
                    HatId = ho.HId,
                    Title = $"Order {ho.OId}",
                    HatName = ho.Hat?.Name ?? "",
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
                    var dayHatOrders = hatOrders
                    .Where(h => h.Date == current.Date)
                    .ToList();

                    var cell = new CalendarCellViewModel
                    {
                        Date = current,
                        IsCurrentMonth = current.Month == selectedMonth,
                        IsToday = current.Date == today
                    };

                    foreach(var ho in dayHatOrders)
{
                        cell.Events.Add(new CalendarEventViewModel
                        {
                            OrderId = ho.OId,
                            HatId = ho.HId,
                            Title = $"Order {ho.OId}",
                            HatName = ho.Hat?.Name ?? "",
                            Status = ho.Status,
                            ColorClass = GetColorClass(ho.Order, ho.Status),

                            EmployeeId = ho.EId ?? 0,
                            EmployeeName = ho.Employee?.Name ?? ""
                        });
                    }

                    weekRow.Days.Add(cell);
                    current = current.AddDays(1);
                }

                model.Weeks.Add(weekRow);
            }

            model.Employees = await _context.Employees
                .Select(e => new EmployeeViewModel
                {
                    Id = e.EId,
                    Name = e.Name
                })
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTaskToDate(
            int orderId,
            int hatId,
            int employeeId,
            DateTime date,
            bool personal = false,
            int? year = null,
            int? month = null)
            {
            var hatOrder = await _context.HatOrders
                .FirstOrDefaultAsync(h => h.OId == orderId && h.HId == hatId);

            if (hatOrder == null)
                return NotFound();

            hatOrder.Date = date;
            hatOrder.Status = "Påbörjad";
            hatOrder.EId = employeeId;


            await _hatOrderRepository.ChangeToStartedAsync(hatOrder);

            return RedirectToAction(nameof(Index), new
            {
                personal,
                year = year ?? date.Year,
                month = month ?? date.Month
            });
        }

        [HttpPost]
        public async Task<IActionResult> UnassignTask(
    int orderId,
    int hatId,
    bool personal = false,
    int? year = null,
    int? month = null)
        {
            var hatOrder = await _context.HatOrders
                .FirstOrDefaultAsync(h => h.OId == orderId && h.HId == hatId);

            if (hatOrder == null)
                return NotFound();

            hatOrder.Date = null;
            hatOrder.EId = null;
            hatOrder.Status = "Ej Påbörjad";

            await _hatOrderRepository.UpdateAsync(hatOrder);

            return RedirectToAction(nameof(Index), new
            {
                personal,
                year,
                month
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