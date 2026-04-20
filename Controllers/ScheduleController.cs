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


            // ✅ HÄMTA SCHEMA
            var schedulesQuery = _context.HatSchedule
                .Include(s => s.Employee)
                .Include(s => s.HatOrder)
                    .ThenInclude(h => h.Hat)
                .Include(s => s.HatOrder.Order)
                .AsQueryable();

            if (personal)
            {
                schedulesQuery = schedulesQuery.Where(s => s.EmployeeId == currentEmployeeId);
            }

            var schedules = await schedulesQuery.ToListAsync();

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

            var unscheduled = allHatOrders.Where(ho => ho.Status == "Ej påbörjad").ToList();
            

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
                    var daySchedules = schedules
                        .Where(s => s.Date.Date == current.Date)
                        .ToList();

                    var cell = new CalendarCellViewModel
                    {
                        Date = current,
                        IsCurrentMonth = current.Month == selectedMonth,
                        IsToday = current.Date == today
                    };

                    foreach (var s in daySchedules)
                    {
                        cell.Events.Add(new CalendarEventViewModel
                        {
                            OrderId = s.HatOrder.OId,
                            HatId = s.HatOrder.HId,
                            Title = $"Order {s.HatOrder.OId}",
                            HatName = s.HatOrder.Hat?.Name ?? "",
                            Status = s.Status,
                            ColorClass = GetColorClass(s.HatOrder.Order, s.Status),

                            // ✅ NYTT
                            EmployeeId = s.EmployeeId,
                            EmployeeName = s.Employee.Name
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


            _hatOrderRepository.UpdateAsync(hatOrder);

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