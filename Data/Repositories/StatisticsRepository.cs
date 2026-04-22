using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class StatisticsRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly HatOrderRepository _hatOrderRepository;

        public StatisticsRepository(ApplicationDbContext db, HatOrderRepository hatOrderRepository)
        {
            _db = db;
            _hatOrderRepository = hatOrderRepository;
        }

        public async Task<int> getAmoutTotalSoldHats()
        {
            var totalSoldHats = await _db.HatOrders
                .Where(ho => ho.Status == "Skickad")
                .ToListAsync();

            var includeAllTotalSoldHats = 0;

            foreach (var ho in totalSoldHats)
            {
                includeAllTotalSoldHats += ho.Amount;
            }
            return includeAllTotalSoldHats;
        }

        public async Task<int> getTotalRevenue()
        {
            var totalRevenue = await _db.HatOrders
                .Where(ho => ho.Status == "Skickad")
                .Include(ho => ho.Hat)
                .ToListAsync();
            int totalRevenueAmount = (int)totalRevenue.Sum(ho => ho.Hat.Price * ho.Amount);
            return totalRevenueAmount;


        }
    }
}
