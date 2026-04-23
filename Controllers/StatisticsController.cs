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

        public async Task<IActionResult> Index(string period = "all", string sort = "sales", string direction = "desc")
        {
            var revenue = await _statisticsRepository.getTotalRevenue(period);
            var totalSoldHats = await _statisticsRepository.getAmoutTotalSoldHats(period);
            var allHats = await _statisticsRepository.GetAllHatsAsync();
            var allHatOrders = await _statisticsRepository.GetAllHatOrdersAsync(period);


            var stats = allHatOrders
                .GroupBy(ho => new { ho.HId, ho.Hat.Name, ho.Hat.Price })
                .Select(g => new HatStatisticsRow
                {
                    HatId = g.Key.HId,
                    Name = g.Key.Name,
                    Price = g.Key.Price,
                    Quantity = g.Sum(x => x.Amount),
                    Sales = g.Sum(x => x.Amount) * g.Key.Price
                });

            stats = sort switch
            {
                "quantity" => direction == "asc"
                    ? stats.OrderBy(x => x.Quantity)
                    : stats.OrderByDescending(x => x.Quantity),

                "price" => direction == "asc"
                    ? stats.OrderBy(x => x.Price)
                    : stats.OrderByDescending(x => x.Price),

                "name" => direction == "asc"
                    ? stats.OrderBy(x => x.Name)
                    : stats.OrderByDescending(x => x.Name),

                _ => direction == "asc"
                    ? stats.OrderBy(x => x.Sales)
                    : stats.OrderByDescending(x => x.Sales)
            };

            ViewBag.Sort = sort;
            ViewBag.Direction = direction;

            var result = stats.ToList();


            var viewModel = new StatisticsViewModel
            {
                TotalSoldHats = totalSoldHats,
                TotalRevenue = revenue,
                hatStats = result
            };

            ViewBag.CurrentPeriod = period;

            return View(viewModel);





        }


    }
}
