using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class MaterialOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public MaterialOrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(MaterialOrder materialOrder)
        {
            _db.MaterialOrders.Add(materialOrder);
            await _db.SaveChangesAsync();
        }

        public async Task<MaterialOrder?> GetByIdAsync(int id)
        {
            return await _db.MaterialOrders
                .FirstOrDefaultAsync(mo => mo.MoId == id);
        }

        public async Task<List<MaterialOrder>> GetAllAsync()
        {
            return await _db.MaterialOrders
               .ToListAsync();
        }

        public async Task UpdateAsync(MaterialOrder materialOrder)
        {
            _db.MaterialOrders.Update(materialOrder);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(MaterialOrder materialOrder)
        {
            _db.MaterialOrders.Remove(materialOrder);
            await _db.SaveChangesAsync();
        }
    }
}
