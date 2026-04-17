using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var customers = _context.Customers.ToList();
            return View(customers);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("EmployeeId") != null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerCreateViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string name = model.Name.Trim();

            bool nameExists = _context.Customers
                .Any(c => c.Name.ToLower() == name.ToLower());

            if (nameExists)
            {
                ModelState.AddModelError("Name", "Namnet är redan upptaget.");
                return View(model);
            }

            var customer = new Customer
            {
                Name = model.Name.Trim(),
                Adress = model.Adress.Trim(),
                PhoneNr = model.PhoneNr.Trim(),
                Email = model.Email.Trim(),
                Country = model.Country.Trim(),
                City = model.City.Trim(),
                Language = model.Language.Trim()
            };

            _context.Customers.Add(customer);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Fel vid sparande av kund.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CId == id);
            if (customer == null)
            {
                return NotFound();
            }
            customer.Name = "";
            customer.Adress = "";
            customer.PhoneNr = "";
            customer.Email = "";
            customer.Country = "";
            customer.City = "";
            customer.Language = "";

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            // Fetch the customer (was fetching an employee and referencing undefined 'customer')
            var customer = _context.Customers.FirstOrDefault(c => c.CId == id);
            if (customer == null)
            {
                return NotFound();
            }

            var model = new CustomerEditViewModel
            {
                CId = customer.CId,
                Name = customer.Name,
                Adress = customer.Adress,
                PhoneNr = customer.PhoneNr,
                Email = customer.Email,
                Country = customer.Country,
                City = customer.City,
                Language = customer.Language
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CustomerEditViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the customer to update (was checking Employees and using model.EId)
            var customer = _context.Customers.FirstOrDefault(c => c.CId == model.CId);
            if (customer == null)
            {
                return NotFound();
            }

            string name = (model.Name ?? string.Empty).Trim();

            bool nameExists = _context.Customers.Any(c =>
                c.CId != model.CId &&
                c.Name.ToLower() == name.ToLower());

            if (nameExists)
            {
                ModelState.AddModelError("Name", "Kundnamnet är redan upptaget.");
                return View(model);
            }

            customer.Name = model.Name?.Trim();
            customer.Adress = model.Adress?.Trim();
            customer.PhoneNr = model.PhoneNr?.Trim();
            customer.Email = model.Email?.Trim();
            customer.Country = model.Country?.Trim();
            customer.City = model.City?.Trim();
            customer.Language = model.Language?.Trim();

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Fel vid uppdatering av kund.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}