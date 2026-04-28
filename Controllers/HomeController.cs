using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var today = DateTime.Today;

            int diff = ((int)today.DayOfWeek + 6) % 7;
            var weekStart = today.AddDays(-diff);
            var weekEnd = weekStart.AddDays(7);

            // Dagens order-/hattuppgifter för inloggad användare
            var todayHatOrders = await _context.HatOrders
                .Include(h => h.Hat)
                .Include(h => h.Order)
                .Include(h => h.Employee)
                .Where(h => h.Date.HasValue
                         && h.Date.Value.Date == today
                         && h.Status != "Completed"
                         && h.Status != "Shipped"
                         && h.Status != "Returned"
                         && h.EId == currentEmployeeId.Value)
                .ToListAsync();

            // Dagens aktiviteter, till exempel fika, städa, möte
            var todayActivities = await _context.CustomActivities
                .Include(a => a.Employee)
                .Where(a => a.Date.HasValue
                         && a.Date.Value.Date == today
                         && a.EId == currentEmployeeId.Value)
                 .OrderBy(a => a.Time)
                .ToListAsync();

            // Kvartalets datumintervall
            int currentQuarter = (today.Month - 1) / 3 + 1;
            int quarterStartMonth = (currentQuarter - 1) * 3 + 1;

            var quarterStart = new DateTime(today.Year, quarterStartMonth, 1);
            var quarterEnd = quarterStart.AddMonths(3);

            // Dagens försäljning
            var todaySales = await _context.Orders
                .Where(o => o.Status == "Shipped"
                         && o.SentDate >= today
                         && o.SentDate < today.AddDays(1))
                .SumAsync(o => o.Price);

            var weekSales = await _context.Orders
                .Where(o => o.Status == "Shipped"
                         && o.SentDate >= weekStart
                         && o.SentDate < weekEnd)
                .SumAsync(o => o.Price);

            // Kvartalets försäljning
            var quarterSales = await _context.Orders
                .Where(o => o.Status == "Shipped"
                         && o.SentDate >= quarterStart
                         && o.SentDate < quarterEnd)
                .SumAsync(o => o.Price);

            // Lista för Dagens schema = bara CustomActivities
            var todayActivitiesList = new List<TodayScheduleItemViewModel>();

            foreach (var activity in todayActivities)
            {
                todayActivitiesList.Add(new TodayScheduleItemViewModel
                {
                    Title = activity.Name,
                    Type = "Schema",
                    Status = "Planerad",
                    EmployeeName = activity.Employee?.Name ?? "",
                    Amount = 1,
                    Time = activity.Time
                });
            }

            // Lista för Dagens arbetsuppgifter = bara HatOrders
            var todayOrderTasks = new List<TodayScheduleItemViewModel>();

            foreach (var task in todayHatOrders)
            {
                todayOrderTasks.Add(new TodayScheduleItemViewModel
                {
                    Title = task.Hat?.Name ?? "Hattuppgift",
                    Type = $"Order {task.OId}",
                    Status = task.Status,
                    EmployeeName = task.Employee?.Name ?? "",
                    Amount = task.Amount
                });
            }

            var model = new DashViewModel
            {
                TodaySales = todaySales,
                WeekSales = weekSales,
                QuarterSales = quarterSales,
                TodayActivities = todayActivitiesList,
                TodayOrderTasks = todayOrderTasks
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}