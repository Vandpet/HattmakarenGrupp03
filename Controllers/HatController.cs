using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            var vm = new HatCreateViewModel();
            vm.AvailableMaterials = GetMockMaterials(); // Fyller på med test-material
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HatCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableMaterials = GetMockMaterials();
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

                // Skapar kopplingen mellan hatt och valda material
                Materials = vm.SelectedMaterialIds.Select(id => new HatMaterial
                {
                    MId = id
                }).ToList()
            };

            // Vi kommenterar bort sparandet för att undvika Foreign Key-fel vid test
            // await _hatRepository.AddAsync(hat);

            return RedirectToAction("Index", "Home");
        }

        // Hjälpmetod för att simulera databas-material
        private List<MaterialSelectionViewModel> GetMockMaterials()
        {
            return new List<MaterialSelectionViewModel>
            {
                new MaterialSelectionViewModel { Id = 1, Name = "Svart Filt" },
                new MaterialSelectionViewModel { Id = 2, Name = "Sidenband (Rött)" },
                new MaterialSelectionViewModel { Id = 3, Name = "Strutsfjäder" },
                new MaterialSelectionViewModel { Id = 4, Name = "Läderrem" }
            };
        }
    }
}