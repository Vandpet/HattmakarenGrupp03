using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Services;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class HatController : Controller
    {
        private readonly HatRepository _hatRepository;
        private readonly OrderRepository _orderRepository;
        private readonly HatOrderRepository _hatOrderRepository;
        private readonly FileService _fileService;

        public HatController(
            HatRepository hatRepository,
            OrderRepository orderRepository,
            HatOrderRepository hatOrderRepository,
            FileService fileService)
        {
            _hatRepository = hatRepository;
            _orderRepository = orderRepository;
            _hatOrderRepository = hatOrderRepository;
            _fileService = fileService;
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("EmployeeId") != null;
        }

        private void PopulateKnOptions(HatCreateViewModel vm)
        {
            vm.KN_options = _hatRepository.GetKNStringList();
        }

        private void EnsureAtLeastOneMaterialRow(HatCreateViewModel vm)
        {
            if (vm.Materials == null || !vm.Materials.Any())
            {
                vm.Materials = new List<MaterialCreateViewModel>
                {
                    new MaterialCreateViewModel()
                };
            }
        }

        private static (string? KNNumber, string? KNDescription) ParseKN(string? selectedKN)
        {
            if (string.IsNullOrWhiteSpace(selectedKN))
            {
                return (null, null);
            }

            var parts = selectedKN.Split('-', 2);
            var number = parts[0].Trim();
            var description = parts.Length > 1 ? parts[1].Trim() : "";

            return (number, description);
        }

        public IActionResult Create()
        {
            // Säkerhetskoll: Om sessionen gått ut eller man inte är inloggad
            var currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (currentEmployeeId == null) return RedirectToAction("Login", "Auth");

            var vm = new HatCreateViewModel
            {
                Materials = new List<MaterialCreateViewModel>
                {
                    new MaterialCreateViewModel()
                }
            };

            PopulateKnOptions(vm);
            ViewBag.IsEdit = false;

            return View("Create", vm);
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
                }

                PopulateKnOptions(vm);
                EnsureAtLeastOneMaterialRow(vm);
                ViewBag.IsEdit = false;

                return View("Create", vm);
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
                    PopulateKnOptions(vm);
                    EnsureAtLeastOneMaterialRow(vm);
                    ViewBag.IsEdit = false;

                    return View("Create", vm);
                }
            }

            var (knNumber, knDescription) = ParseKN(vm.SelectedKN);

            var hat = new Hat
            {
                Name = vm.Name,
                Price = vm.Price,
                Size = vm.Size,
                PicturePath = picturePath ?? "",
                StandardHat = vm.StandardHat,
                Description = vm.Description,
                KN_Number = knNumber,
                KN_Description = knDescription,
                Materials = vm.Materials?
                    .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name))
                    .Select(m => new HatMaterial
                    {
                        Material = new Material
                        {
                            Name = m.Name,
                            Amount = (double)m.Amount,
                            MeasuringUnits = m.MeasuringUnits,
                            Price = m.Price
                        }
                    })
                    .ToList() ?? new List<HatMaterial>()
            };

            await _hatRepository.AddAsync(hat);

            return RedirectToAction("Index", "Hat");
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

        public async Task<IActionResult> Details(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var hat = await _hatRepository.GetByIdAsync(id);
            if (hat == null)
            {
                return NotFound();
            }

            return View(hat);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var hat = await _hatRepository.GetByIdAsync(id);
            if (hat == null)
            {
                return NotFound();
            }

            var vm = new HatCreateViewModel
            {
                HId = hat.HId,
                Name = hat.Name,
                Price = hat.Price,
                Size = hat.Size,
                StandardHat = hat.StandardHat,
                PicturePath = hat.PicturePath,
                Description = hat.Description,
                SelectedKN = string.IsNullOrWhiteSpace(hat.KN_Number)
                    ? null
                    : $"{hat.KN_Number} - {hat.KN_Description}",
                Materials = hat.Materials?
                    .Select(hm => new MaterialCreateViewModel
                    {
                        Name = hm.Material?.Name ?? "",
                        Amount = (decimal)(hm.Material?.Amount ?? 0),
                        MeasuringUnits = hm.Material?.MeasuringUnits ?? "",
                        Price = hm.Material?.Price ?? 0
                    })
                    .ToList() ?? new List<MaterialCreateViewModel>()
            };

            PopulateKnOptions(vm);
            EnsureAtLeastOneMaterialRow(vm);
            ViewBag.IsEdit = true;

            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HatCreateViewModel vm)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id != vm.HId)
            {
                return BadRequest();
            }

            var hat = await _hatRepository.GetByIdAsync(id);
            if (hat == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateKnOptions(vm);
                EnsureAtLeastOneMaterialRow(vm);
                ViewBag.IsEdit = true;

                return View("Create", vm);
            }

            hat.Name = vm.Name;
            hat.Price = vm.Price;
            hat.Size = vm.Size;
            hat.StandardHat = vm.StandardHat;
            hat.Description = vm.Description;

            var (knNumber, knDescription) = ParseKN(vm.SelectedKN);
            hat.KN_Number = knNumber;
            hat.KN_Description = knDescription;

            if (vm.ImageFile != null)
            {
                try
                {
                    var picturePath = await _fileService.SaveImageAsync(vm.ImageFile);
                    hat.PicturePath = picturePath ?? hat.PicturePath;
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PopulateKnOptions(vm);
                    EnsureAtLeastOneMaterialRow(vm);
                    ViewBag.IsEdit = true;

                    return View("Create", vm);
                }
            }

            hat.Materials.Clear();

            foreach (var m in vm.Materials?
                .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name))
                ?? Enumerable.Empty<MaterialCreateViewModel>())
            {
                hat.Materials.Add(new HatMaterial
                {
                    Material = new Material
                    {
                        Name = m.Name,
                        Amount = (double)m.Amount,
                        MeasuringUnits = m.MeasuringUnits,
                        Price = m.Price
                    }
                });
            }

            await _hatRepository.UpdateAsync(hat);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateModal()
        {
            var vm = new HatCreateViewModel
            {
                StandardHat = false
            };

            PopulateKnOptions(vm);
            EnsureAtLeastOneMaterialRow(vm);

            return PartialView("_CreateHatModalPartial", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(HatCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateKnOptions(vm);
                EnsureAtLeastOneMaterialRow(vm);

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
                    PopulateKnOptions(vm);
                    EnsureAtLeastOneMaterialRow(vm);

                    return PartialView("_CreateHatModalPartial", vm);
                }
            }

            var (knNumber, knDescription) = ParseKN(vm.SelectedKN);

            var hat = new Hat
            {
                Name = vm.Name,
                Price = vm.Price,
                Size = vm.Size,
                PicturePath = picturePath ?? "",
                StandardHat = false,
                Description = vm.Description,
                KN_Number = knNumber,
                KN_Description = knDescription,
                Materials = vm.Materials?
                    .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name))
                    .Select(m => new HatMaterial
                    {
                        Material = new Material
                        {
                            Name = m.Name,
                            Amount = (double)m.Amount,
                            MeasuringUnits = m.MeasuringUnits,
                            Price = m.Price
                        }
                    })
                    .ToList() ?? new List<HatMaterial>()
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

        public async Task<IActionResult> hatView(int hId, int oId, int amount)
        {
            return await View(hId, oId);
        }

        public async Task<IActionResult> View(int hId, int oId)
        {
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var hatOrder = await _hatOrderRepository.GetByIdAsync(hId, oId);
            var hat = await _hatRepository.GetByIdAsync(hId);

            if (hat == null)
            {
                return NotFound();
            }

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
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var hatOrder = await _hatOrderRepository.GetByIdAsync(hId, oId);
            await _hatOrderRepository.ChangeToReturnedAsync(hatOrder);

            return RedirectToAction("Details", "Order", new { oId });
        }
    }
}