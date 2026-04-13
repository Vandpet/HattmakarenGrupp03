using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return View("Create", new HatCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HatCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", vm);
            }

            var hat = new Hat
            {
                Name = vm.Name,
                Price = vm.Price,
                Size = vm.Size,
                PicturePath = vm.PicturePath ?? "",

                // Hårdkodat / default
                Status = vm.Status ?? "Accepted",
                StandardHat = vm.StandardHat,

        //        Materials = new List<Material>
        //{
        //    new Material
        //    {
        //        Name = "Standardmaterial",
        //        Amount = 1,
        //        MeasuringUnits = "st",
        //        Price = 10,
        //        Hats = new List<Hat>(),
        //        MaterialOrders = new List<MaterialOrder>()
        //    }
        //}
            };

            await _hatRepository.AddAsync(hat);

            return RedirectToAction(nameof(Index));
        }
    }
}
