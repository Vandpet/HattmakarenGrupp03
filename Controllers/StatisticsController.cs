using Microsoft.AspNetCore.Mvc;
using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Models.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;


namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly StatisticsRepository _statisticsRepository;

        public StatisticsController(StatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var revenue = await _statisticsRepository.getTotalRevenue();
            var totalSoldHats = await _statisticsRepository.getAmoutTotalSoldHats();

            Debug.WriteLine("Total sold hats: " + totalSoldHats);
            Debug.WriteLine("Total revenue: " + revenue);

            var viewModel = new StatisticsViewModel
            {
                TotalSoldHats = totalSoldHats,
                TotalRevenue = revenue
            };

            return View(viewModel);

            



        }

        
    }
}
