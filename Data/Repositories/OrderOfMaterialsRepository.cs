using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class OrderOfMaterialsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderOfMaterialsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(OrderOfMaterials orderOfMaterials)
        {
            _db.OrderOfMaterials.Add(orderOfMaterials);
            await _db.SaveChangesAsync();
        }

        public async Task<OrderOfMaterials?> GetByIdAsync(int OId, int MoId)
        {
            return await _db.OrderOfMaterials
                .FirstOrDefaultAsync(om => om.OId == OId && om.MoId == MoId);
        }

        public async Task<List<OrderOfMaterials>> GetAllAsync()
        {
            return await _db.OrderOfMaterials
               .ToListAsync();
        }

        public async Task UpdateAsync(OrderOfMaterials orderOfMaterials)
        {
            _db.OrderOfMaterials.Update(orderOfMaterials);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrderOfMaterials orderOfMaterials)
        {
            _db.OrderOfMaterials.Remove(orderOfMaterials);
            await _db.SaveChangesAsync();
        }
    }
}
