using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class HatController : Controller
    {
        private readonly HatRepository _hatRepository;

        public HatController(HatRepository hatRepository)
        {
            _hatRepository = hatRepository;
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

            var hat = new Hat
            {
                Name = vm.Name,
                Price = vm.Price,
                Size = vm.Size,
                PicturePath = vm.PicturePath ?? "",
                Status = vm.Status ?? "Accepted",
                StandardHat = vm.StandardHat,

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
    }
}