using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class HatMaterialRepository
    {
        private readonly ApplicationDbContext _db;

        public HatMaterialRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(HatMaterial hatMaterial)
        {
            _db.HatMaterials.Add(hatMaterial);
            await _db.SaveChangesAsync();
        }

        public async Task<HatMaterial?> GetByIdAsync(int HId, int MId)
        {
            return await _db.HatMaterials
                .FirstOrDefaultAsync(hm => hm.HId == HId && hm.MId == MId);
        }

        public async Task<List<HatMaterial>> GetAllAsync()
        {
            return await _db.HatMaterials
               .ToListAsync();
        }

        public async Task UpdateAsync(HatMaterial hatMaterial)
        {
            _db.HatMaterials.Update(hatMaterial);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(HatMaterial hatMaterial)
        {
            _db.HatMaterials.Remove(hatMaterial);
            await _db.SaveChangesAsync();
        }
    }
}
