using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Employee> _passwordHasher;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Employee>();
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("EmployeeId") != null)
            {
                return RedirectToAction("Index", "Order");
            }

            ViewBag.NoUsersExist = !_context.Employees.Any();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string username = model.Username.Trim().ToLower();

            var employee = _context.Employees
                .FirstOrDefault(e => e.Username.ToLower() == username);

            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Fel användarnamn eller lösenord.");
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(
                employee,
                employee.PasswordHash,
                model.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Fel användarnamn eller lösenord.");
                return View(model);
            }

            HttpContext.Session.SetInt32("EmployeeId", employee.EId);
            HttpContext.Session.SetString("Username", employee.Username);
            HttpContext.Session.SetInt32("AccessLevel", employee.accesslevel);
            HttpContext.Session.SetString("Name", employee.Name);

            return RedirectToAction("Index", "Order");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}