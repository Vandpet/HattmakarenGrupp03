using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class HatOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public HatOrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(HatOrder hatOrder)
        {
            _db.HatOrders.Add(hatOrder);
            await _db.SaveChangesAsync();
        }

        public async Task<HatOrder?> GetByIdAsync(int HId, int OId)
        {
            return await _db.HatOrders
                .FirstOrDefaultAsync(ho => ho.HId == HId && ho.OId == OId);
        }

        public async Task<List<HatOrder>> GetAllAsync()
        {
            return await _db.HatOrders
               .ToListAsync();
        }

        public async Task UpdateAsync(HatOrder hatOrder)
        {
            _db.HatOrders.Update(hatOrder);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(HatOrder hatOrder)
        {
            _db.HatOrders.Remove(hatOrder);
            await _db.SaveChangesAsync();
        }

        //Specialmetoder
        public async Task AssignEmployee(HatOrder hatOrder, int EId)
        {
            hatOrder.EId = EId;
            //Sätt status till "Assigned" när en anställd tilldelas eller något liknande
            await _db.SaveChangesAsync();
        }
    }
}
