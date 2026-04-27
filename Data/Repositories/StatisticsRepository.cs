using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class StatisticsRepository
    {

        private readonly ApplicationDbContext _db;
        public StatisticsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> getAmoutTotalSoldHats(string period)
        {
            DateTime now = DateTime.Now;
            DateTime limit = now;

            if (period == "month")
            {
                // Första dagen i innevarande månad
                limit = new DateTime(now.Year, now.Month, 1);
            }
            else if (period == "quarter")
            {
                // Räknar ut startmånaden för kvartalet (1, 4, 7 eller 10)
                int quarterStartMonth = ((now.Month - 1) / 3) * 3 + 1;
                limit = new DateTime(now.Year, quarterStartMonth, 1);
            }
            else if (period == "year")
            {
                // Första dagen på året
                limit = new DateTime(now.Year, 1, 1);
            }

            var totalSoldHats = await _db.HatOrders
                    .Where(ho => ho.Status == "Shipped")
                    .ToListAsync(); ;
            
            if (period != "all")
            {
                totalSoldHats = totalSoldHats
                    .Where(ho => ho.Date >= limit)
                    .ToList();
            }

            var includeAllTotalSoldHats = 0;

            foreach (var ho in totalSoldHats)
            {
                includeAllTotalSoldHats += ho.Amount;
            }
            return includeAllTotalSoldHats;
        }

        public async Task<int> getTotalRevenue(string period = "all")
        {
            var totalRevenue = await _db.HatOrders
                .Where(ho => ho.Status == "Shipped")
                .Include(ho => ho.Hat)
                .ToListAsync();

            DateTime now = DateTime.Now;
            DateTime limit = now;

            if (period == "month")
            {
                // Första dagen i innevarande månad
                limit = new DateTime(now.Year, now.Month, 1);
            }
            else if (period == "quarter")
            {
                // Räknar ut startmånaden för kvartalet (1, 4, 7 eller 10)
                int quarterStartMonth = ((now.Month - 1) / 3) * 3 + 1;
                limit = new DateTime(now.Year, quarterStartMonth, 1);
            }
            else if (period == "year")
            {
                // Första dagen på året
                limit = new DateTime(now.Year, 1, 1);
            }

            int totalRevenueAmount = 0;
            
            if (period != "all")
            {
                totalRevenueAmount = (int)totalRevenue.Where(tr => tr.Date >= limit).Sum(ho => ho.Hat.Price * ho.Amount);
            }else
            {
                totalRevenueAmount = (int)totalRevenue.Sum(ho => ho.Hat.Price * ho.Amount);
            }

            return totalRevenueAmount;

        }

        public async Task<List<Hat>> GetAllHatsAsync()
        {
            return await _db.Hats
                .Include(h => h.Materials)
                .ToListAsync();
        }

        public async Task<List<HatOrder>> GetAllHatOrdersAsync(string period)
        {
            DateTime now = DateTime.Now;
            DateTime limit = now;

            if (period == "month")
            {
                // Första dagen i innevarande månad
                limit = new DateTime(now.Year, now.Month, 1);
            }
            else if (period == "quarter")
            {
                // Räknar ut startmånaden för kvartalet (1, 4, 7 eller 10)
                int quarterStartMonth = ((now.Month - 1) / 3) * 3 + 1;
                limit = new DateTime(now.Year, quarterStartMonth, 1);
            }
            else if (period == "year")
            {
                // Första dagen på året
                limit = new DateTime(now.Year, 1, 1);
            }

            if (period != "all")
            {
                return await _db.HatOrders
                    .Where(ho => ho.Date >= limit)
                    .ToListAsync();
            }

            return await _db.HatOrders.ToListAsync();
        }

    }
}
