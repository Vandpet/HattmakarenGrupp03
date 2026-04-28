using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Globalization;

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

        private bool IsAdmin()
        {
            int? employeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (employeeId == null)
                return false;

            var employee = _context.Employees.FirstOrDefault(e => e.EId == employeeId.Value);

            return employee != null && employee.accesslevel >= 10;
        }

		public async Task<IActionResult> Index(bool personal = false, DateTime? targetDate = null, string viewMode = "month")
		{
			if (HttpContext.Session.GetInt32("EmployeeId") == null) return RedirectToAction("Login", "Auth");

			int currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId")!.Value;

			DateTime selectedDate = targetDate ?? DateTime.Today;
			viewMode = viewMode.ToLower();

			DateTime startDate;
			int weeksToGenerate;
			int daysInWeek;

			if (viewMode == "day")
			{
				startDate = selectedDate.Date;
				weeksToGenerate = 1;
				daysInWeek = 1;
			}
			else if (viewMode == "week")
			{
				int diff = ((int)selectedDate.DayOfWeek + 6) % 7;
				startDate = selectedDate.AddDays(-diff).Date;
				weeksToGenerate = 1;
				daysInWeek = 7;
			}
			else
			{
				var firstDay = new DateTime(selectedDate.Year, selectedDate.Month, 1);
				int diff = ((int)firstDay.DayOfWeek + 6) % 7;
				startDate = firstDay.AddDays(-diff).Date;
				weeksToGenerate = 6;
				daysInWeek = 7;
			}


			int weekNumber = ISOWeek.GetWeekOfYear(DateTime.Today);

            //int weeknumber = calender.GetWeekOfYear(new DateTime(selectedYear, selectedMonth, 1), 
            //    CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

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

            var activitiesQuery = _context.CustomActivities
                .Include(a => a.Employee)
                .AsQueryable();

            if (personal)
            {
                activitiesQuery = activitiesQuery.Where(a => a.EId == currentEmployeeId);
            }

            var activities = await activitiesQuery
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Time)
                .ToListAsync();


            // HÄMTA ALLA HATORDERS (för oschemalagda)
            var allHatOrders = await _context.HatOrders
				.Include(h => h.Hat)
				.Include(h => h.Order)
                     .ThenInclude(o => o.Customer)
				.ToListAsync();

			//var scheduledHatOrderIds = schedules.Select(s => s.HatOrderId).ToHashSet();

			var model = new MonthScheduleViewModel
			{
				IsPersonal = personal,
				Year = selectedDate.Year,
				Month = selectedDate.Month,
				SelectedDate = selectedDate,
				ViewMode = viewMode,
				IsAdmin = IsAdmin(),
				WeekNumber = weekNumber
			};


			var unscheduled = allHatOrders
				.Where(ho => ho.Status == "Not Started")
				.OrderBy(ho => ho.Order?.PrelDeliveryDate)
				.ThenBy(ho => ho.OId)
				.ToList();



            foreach (var ho in unscheduled)
            {
                model.UnscheduledTasks.Add(new UnscheduledTaskViewModel
                {
                    OrderId = ho.OId,
                    HatId = ho.HId,
                    Title = $"Order {ho.OId}",
                    HatName = ho.Hat?.Name ?? "",
                    Status = ho.Status,
                    ColorClass = GetColorClass(ho.Order, ho.Status),
                    Amount = ho.Amount,
                    PrelDeliveryDate = ho.Order?.PrelDeliveryDate,
                    CustomerName = ho.Order?.Customer?.Name ?? "",
                    Description = ho.Order?.Description ?? ""
                });
            }

            DateTime current = startDate;

			for (int week = 0; week < weeksToGenerate; week++)
			{
                var weekStartDate = current;
                var weekRow = new WeekRowViewModel
                {
                    WeekNumber = ISOWeek.GetWeekOfYear(weekStartDate)
                };

				for (int day = 0; day < daysInWeek; day++)
				{
                    var dayHatOrders = hatOrders
                    .Where(h => h.Date == current.Date && 
					h.Status != "Returned" &&
					h.Status != "Shipped")
                    .ToList();

					var cell = new CalendarCellViewModel
					{
						Date = current,
						IsCurrentMonth = current.Month == selectedDate.Month, 
						IsToday = current.Date == DateTime.Today              
					};

					foreach (var ho in dayHatOrders)
                    {
                        cell.Events.Add(new CalendarEventViewModel
                        {
                            OrderId = ho.OId,
                            HatId = ho.HId,
                            Title = $"Order {ho.OId}",
                            HatName = ho.Hat?.Name ?? "",
                            Status = ho.Status,
                            ColorClass = GetColorClass(ho.Order, ho.Status),
                            Amount = ho.Amount,
                            EmployeeId = ho.EId ?? 0,
                            EmployeeName = ho.Employee?.Name ?? "",
                            PrelDeliveryDate = ho.Order?.PrelDeliveryDate
                        });
                    }

                    var dayActivities = activities
                            .Where(a => a.Date.HasValue && a.Date.Value.Date == current.Date)
                            .OrderBy(a => a.Time)
                            .ToList();

                    foreach (var activity in dayActivities)
                    {
                        cell.Events.Add(new CalendarEventViewModel
                        {
                            ActivityId = activity.AId,
                            EventType = "Activity",

                            OrderId = 0,
                            HatId = 0,
                            Title = activity.Name,
                            HatName = activity.Name,
                            Status = "Arbetsuppgift",
                            ColorClass = "order-default",
                            Amount = 1,
                            EmployeeId = activity.EId,
                            EmployeeName = activity.Employee?.Name ?? "",
                            PrelDeliveryDate = null,
                            Time = activity.Time
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
			DateTime? targetDate = null,
			string viewMode = "month")
		{
            if (HttpContext.Session.GetInt32("EmployeeId") == null) return RedirectToAction("Login", "Auth");

            var hatOrder = await _context.HatOrders
				.FirstOrDefaultAsync(h => h.OId == orderId && h.HId == hatId);

			if (hatOrder == null)
				return NotFound();

			hatOrder.Date = date;
			hatOrder.Status = "Started";

			if (IsAdmin())
			{
				hatOrder.EId = employeeId;
			}
			else
			{
				int currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
				hatOrder.EId = currentEmployeeId;
			}

			await _hatOrderRepository.ChangeToStartedAsync(hatOrder);

			return RedirectToAction(nameof(Index), new
			{
				personal,
				targetDate,
				viewMode
			});
		}

		[HttpPost]
		public async Task<IActionResult> UnassignTask(
			int orderId,
			int hatId,
			bool personal = false,
			DateTime? targetDate = null,
			string viewMode = "month")
		{
            if (HttpContext.Session.GetInt32("EmployeeId") == null) return RedirectToAction("Login", "Auth");

            var hatOrder = await _context.HatOrders
				.FirstOrDefaultAsync(h => h.OId == orderId && h.HId == hatId);

			if (hatOrder == null)
				return NotFound();

			await _hatOrderRepository.ChangeToNotStartedAsync(hatOrder);

			return RedirectToAction(nameof(Index), new
			{
				personal,
				targetDate,
				viewMode
			});
		}

		private string GetColorClass(Order? order, string hatOrderStatus)
        {
            if (order == null)
                return "order-default";

            if (order.Status == "Klar" || order.Status == "Completed" || order.Status == "Shipped" ||
                hatOrderStatus == "Klar" || hatOrderStatus == "Completed" || hatOrderStatus == "Shipped")
                return "order-default";

            var daysLeft = (order.PrelDeliveryDate.Date - DateTime.Today).Days;

            if (daysLeft <= 3)
                return "order-red";

            if (daysLeft >= 4 && daysLeft <= 7)
                return "order-orange";

            return "order-default";
        }

		[HttpPost]
		public async Task<IActionResult> MarkDone(
			int orderId,
			int hatId,
			bool personal = false,
			DateTime? targetDate = null,
			string viewMode = "month")
		{
            if (HttpContext.Session.GetInt32("EmployeeId") == null) return RedirectToAction("Login", "Auth");

            var hatOrder = await _context.HatOrders
				.FirstOrDefaultAsync(h => h.OId == orderId && h.HId == hatId);

			if (hatOrder == null)
				return NotFound();

			await _hatOrderRepository.ChangeToCompletedAsync(hatOrder);

			return RedirectToAction(nameof(Index), new
			{
				personal,
				targetDate,
				viewMode
			});
		}

        [HttpPost]
        public async Task<IActionResult> DeleteActivity(
    int activityId,
    bool personal = false,
    DateTime? targetDate = null,
    string viewMode = "month")
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null) return RedirectToAction("Login", "Auth");

            var activity = await _context.CustomActivities
                .FirstOrDefaultAsync(a => a.AId == activityId);

            if (activity == null)
                return NotFound();

            _context.CustomActivities.Remove(activity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new
            {
                personal,
                targetDate,
                viewMode
            });
        }

    }
}