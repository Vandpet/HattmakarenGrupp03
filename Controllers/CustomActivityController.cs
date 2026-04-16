using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class CustomActivityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            bool isAdmin = IsAdmin();

            var activities = _context.CustomActivities
                .Include(a => a.Employee)
                .ToList();

            if (!isAdmin && currentEmployeeId != null)
            {
                activities = activities
                    .Where(a => a.EId == currentEmployeeId.Value)
                    .ToList();
            }

            return View(activities);
        }

        public IActionResult View(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            bool isAdmin = IsAdmin();

            var activity = _context.CustomActivities
                .Include(a => a.Employee)
                .FirstOrDefault(a => a.AId == id);

            if (activity == null)
            {
                return NotFound();
            }

            if (!isAdmin && currentEmployeeId != activity.EId)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(activity);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            var model = new CustomActivityCreateViewModel
            {
                Date = DateTime.Today
            };

            if (IsAdmin())
            {
                model.Employees = _context.Employees
                    .OrderBy(e => e.Name)
                    .Select(e => new SelectListItem
                    {
                        Value = e.EId.ToString(),
                        Text = e.Name
                    })
                    .ToList();
            }
            else if (currentEmployeeId != null)
            {
                model.EId = currentEmployeeId.Value;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomActivityCreateViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (!IsAdmin())
            {
                if (currentEmployeeId == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                model.EId = currentEmployeeId.Value;
            }
            else
            {
                bool employeeExists = _context.Employees.Any(e => e.EId == model.EId);
                if (!employeeExists)
                {
                    ModelState.AddModelError("EId", "Du måste välja en giltig anställd.");
                }
            }

            if (!ModelState.IsValid)
            {
                if (IsAdmin())
                {
                    model.Employees = _context.Employees
                        .OrderBy(e => e.Name)
                        .Select(e => new SelectListItem
                        {
                            Value = e.EId.ToString(),
                            Text = e.Name
                        })
                        .ToList();
                }

                return View(model);
            }

            var activity = new CustomActivity
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim() ?? string.Empty,
                Date = model.Date,
                EId = model.EId
            };

            _context.CustomActivities.Add(activity);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("EmployeeId") != null;
        }

        private bool IsAdmin()
        {
            int? accessLevel = HttpContext.Session.GetInt32("AccessLevel");
            return accessLevel != null && accessLevel >= 10;
        }
    }
}