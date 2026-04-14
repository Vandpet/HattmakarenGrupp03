using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class AssignedOrdersRepository
    {
        private readonly ApplicationDbContext _db;

        public AssignedOrdersRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(AssignedOrders assignedOrder)
        {
            _db.AssignedOrders.Add(assignedOrder);
            await _db.SaveChangesAsync();
        }

        public async Task<AssignedOrders?> GetByIdAsync(int EId, int OId)
        {
            return await _db.AssignedOrders
                .FirstOrDefaultAsync(ao => ao.EId == EId && ao.OId == OId);
        }

        public async Task<List<AssignedOrders>> GetAllAsync()
        {
            return await _db.AssignedOrders
               .ToListAsync();
        }

        public async Task UpdateAsync(AssignedOrders assignedOrder)
        {
            _db.AssignedOrders.Update(assignedOrder);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(AssignedOrders assignedOrder)
        {
            _db.AssignedOrders.Remove(assignedOrder);
            await _db.SaveChangesAsync();
        }
    }
}
