using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Employee> _passwordHasher;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Employee>();
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var employees = _context.Employees.ToList();
            return View(employees);
        }

        public IActionResult View(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (!IsAdmin() && currentEmployeeId != id)
            {
                return RedirectToAction("Index", "Home");
            }

            var employee = _context.Employees.FirstOrDefault(e => e.EId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public IActionResult Create()
        {
            bool hasAnyUsers = _context.Employees.Any();

            if (!hasAnyUsers)
            {
                return View();
            }

            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasSpecialCharacter = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpperCase && hasSpecialCharacter;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            bool hasAnyUsers = _context.Employees.Any();

            if (hasAnyUsers && !IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {

                return View(model);
            }

            if (!IsValidPassword(model.Password))
            {
                ModelState.AddModelError("Password", "Lösenordet måste innehålla minst en stor bokstav, ett specialtecken och vara minst åtta tecken långt.");
                return View(model);
            }

            string username = model.Username.Trim().ToLower();

            bool usernameExists = _context.Employees
                .Any(e => e.Username.ToLower() == username);

            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Användarnamnet är redan upptaget.");
                return View(model);
            }

            var employee = new Employee
            {
                Name = model.Name.Trim(),
                Adress = model.Adress.Trim(),
                PhoneNr = model.PhoneNr.Trim(),
                Email = model.Email.Trim(), 
                accesslevel = model.accesslevel,
                Username = username
            };

            employee.PasswordHash = _passwordHasher.HashPassword(employee, model.Password);

            _context.Employees.Add(employee);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Username", "Användarnamnet är redan upptaget.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (!IsAdmin() && currentEmployeeId != id)
            {
                return RedirectToAction("Index", "Home");
            }

            var employee = _context.Employees.FirstOrDefault(e => e.EId == id);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new EmployeeEditViewModel
            {
                EId = employee.EId,
                Name = employee.Name,
                Adress = employee.Adress,
                PhoneNr = employee.PhoneNr,
                Email = employee.Email, 
                accesslevel = employee.accesslevel,
                Username = employee.Username
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (!IsAdmin() && currentEmployeeId != model.EId)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = _context.Employees.FirstOrDefault(e => e.EId == model.EId);
            if (employee == null)
            {
                return NotFound();
            }

            string username = model.Username.Trim().ToLower();

            bool usernameExists = _context.Employees.Any(e =>
                e.EId != model.EId &&
                e.Username.ToLower() == username);

            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Användarnamnet är redan upptaget.");
                return View(model);
            }

            employee.Name = model.Name.Trim();
            employee.Adress = model.Adress.Trim();
            employee.PhoneNr = model.PhoneNr.Trim();
            employee.Email = model.Email.Trim();
            employee.Username = username;

            if (IsAdmin())
            {
                employee.accesslevel = model.accesslevel;
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (!IsValidPassword(model.NewPassword))
                {
                    ModelState.AddModelError("NewPassword", "Lösenordet måste innehålla minst en stor bokstav, ett specialtecken och vara minst åtta tecken långt.");
                    return View(model);
                }

                employee.PasswordHash = _passwordHasher.HashPassword(employee, model.NewPassword);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (currentEmployeeId == id)
            {
                return RedirectToAction(nameof(Index));
            }

            var employee = _context.Employees.FirstOrDefault(e => e.EId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (currentEmployeeId == id)
            {
                return RedirectToAction(nameof(Index));
            }

            var employee = _context.Employees.FirstOrDefault(e => e.EId == id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.IsDeleted = true;
            employee.DeletedAt = DateTime.Now;

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

        public IActionResult SetupFirstUser()
        {
            if (_context.Employees.Any())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetupFirstUser(EmployeeCreateViewModel model)
        {
            if (_context.Employees.Any())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!IsValidPassword(model.Password))
            {
                ModelState.AddModelError("Password", "Lösenordet måste innehålla minst en stor bokstav, ett specialtecken och vara minst åtta tecken långt.");
                return View(model);
            }

            string username = model.Username.Trim().ToLower();

            bool usernameExists = _context.Employees
                .Any(e => e.Username.ToLower() == username);

            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Användarnamnet är redan upptaget.");
                return View(model);
            }

            var employee = new Employee
            {
                Name = model.Name.Trim(),
                Adress = model.Adress.Trim(),
                PhoneNr = model.PhoneNr.Trim(),
                Email = model.Email.Trim(),
                accesslevel = model.accesslevel,
                Username = username
            };

            employee.PasswordHash = _passwordHasher.HashPassword(employee, model.Password);

            _context.Employees.Add(employee);
            _context.SaveChanges();

            return RedirectToAction("Login", "Auth");
        }

    }
}
