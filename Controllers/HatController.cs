using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using WebApp.Services;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class HatController : Controller
    {
        private readonly HatRepository _hatRepository;
        private readonly OrderRepository _orderRepository;
        private readonly HatOrderRepository _hatOrderRepository;
        private readonly FileService _fileService;


        public HatController(HatRepository hatRepository, OrderRepository orderRepository, HatOrderRepository hatOrderRepository, FileService fileService)
        {
            _hatRepository = hatRepository;
            _orderRepository = orderRepository;
            _hatOrderRepository = hatOrderRepository;
            _fileService = fileService;
        }

        public IActionResult Create()
        {
            //var vm = new HatCreateViewModel();
            //vm.AvailableMaterials = GetMockMaterials(); // Fyller på med test-material
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HatCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Debug.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    Debug.WriteLine(vm.Name);
                    Debug.WriteLine(vm.Materials == null);
                    Debug.WriteLine(vm.Materials?.Count);
                }
                return View("Create", vm);
            }

            string picturePath = "";

            // FILE UPLOAD LOGIC
            if (vm.ImageFile != null)
            {
                try
                {
                    picturePath = await _fileService.SaveImageAsync(vm.ImageFile);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View("Create", vm);
                }
            }

            var hat = new Hat
            {
                Name = vm.Name,
                Price = vm.Price,
                Size = vm.Size,
                PicturePath = picturePath ?? "",
                //Status = vm.Status ?? "Accepted", Patrick tog bort status från Hat-modellen, 
                StandardHat = vm.StandardHat,
                Description = vm.Description,

                Materials = vm.Materials?
                .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name))
                .Select(m => new HatMaterial
                {
                    Material = new Material
                    {
                        Name = m.Name,
                        Amount = m.Amount,
                        MeasuringUnits = m.MeasuringUnits,
                        Price = m.Price
                    }
                }).ToList() ?? new List<HatMaterial>()
            };

            await _hatRepository.AddAsync(hat);

            return RedirectToAction("Index", "Home");
        }
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("EmployeeId") != null;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var hats = await _hatRepository.GetAllAsync();
            return View(hats);
        }

        // Hjälpmetod för att simulera databas-material
        //private List<MaterialSelectionViewModel> GetMockMaterials()
        //{
        //    return new List<MaterialSelectionViewModel>
        //    {
        //        new MaterialSelectionViewModel { Id = 1, Name = "Svart Filt" },
        //        new MaterialSelectionViewModel { Id = 2, Name = "Sidenband (Rött)" },
        //        new MaterialSelectionViewModel { Id = 3, Name = "Strutsfjäder" },
        //        new MaterialSelectionViewModel { Id = 4, Name = "Läderrem" }
        //    };
        //}

        [HttpGet]
        public IActionResult CreateModal()
        {
            var vm = new HatCreateViewModel
            {
                StandardHat = false
            };

            return PartialView("_CreateHatModalPartial", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(HatCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateHatModalPartial", vm);
            }

            string picturePath = "";

            if (vm.ImageFile != null)
            {
                try
                {
                    picturePath = await _fileService.SaveImageAsync(vm.ImageFile);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return PartialView("_CreateHatModalPartial", vm);
                }
            }

            var hat = new Hat
            {
                Name = vm.Name,
                Price = vm.Price,
                Size = vm.Size,
                PicturePath = picturePath ?? "",
                StandardHat = false,
                Description = vm.Description,
                Materials = vm.Materials?
                    .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name))
                    .Select(m => new HatMaterial
                    {
                        Material = new Material
                        {
                            Name = m.Name,
                            Amount = m.Amount,
                            MeasuringUnits = m.MeasuringUnits,
                            Price = m.Price
                        }
                    }).ToList() ?? new List<HatMaterial>()
            };

            await _hatRepository.AddAsync(hat);

            return Json(new
            {
                success = true,
                hId = hat.HId,
                description = hat.Description,
                name = hat.Name,
                price = hat.Price.ToString("0.00")
            });
        }

        public async Task<IActionResult> View(int hId, int oId)
        {
            // 1. Hämta EmployeeId från Sessionen
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            // 2. Säkerhetskoll: Om sessionen gått ut eller man inte är inloggad
            if (currentEmployeeId == null) return RedirectToAction("Login", "Auth");

            var hatOrder = await _hatOrderRepository.GetByIdAsync(hId, oId);

            var hat = await _hatRepository.GetByIdAsync(hId);
            if (hat == null) return NotFound();

            var previousOrder = await _orderRepository.GetOrderByIdWithCustomerAndCreatorAsync(oId);

            var viewModel = new HatViewViewModel
            {
                Hat = hat,
                PreviousOrder = previousOrder,
                HatOrder = hatOrder
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ReturnOrder(int oId, int hId)
        {
            // 1. Hämta EmployeeId från Sessionen
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            // 2. Säkerhetskoll: Om sessionen gått ut eller man inte är inloggad
            if (currentEmployeeId == null) return RedirectToAction("Login", "Auth");

            // 3. Returnera hatten
            var hatOrder = await _hatOrderRepository.GetByIdAsync(hId, oId);
            await _hatOrderRepository.ChangeToReturnedAsync(hatOrder);

            return RedirectToAction("Details", "Order", new { oId });
        }
    }
}