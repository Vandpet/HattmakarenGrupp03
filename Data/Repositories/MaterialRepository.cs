using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class MaterialRepository
    {
        private readonly ApplicationDbContext _db;

        public MaterialRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Material material)
        {
            _db.Materials.Add(material);
            await _db.SaveChangesAsync();
        }

        public async Task<Material?> GetByIdAsync(int id)
        {
            return await _db.Materials
                .FirstOrDefaultAsync(m => m.MId == id);
        }

        public async Task<List<Material>> GetAllAsync()
        {
            return await _db.Materials
               .ToListAsync();
        }

        public async Task UpdateAsync(Material material)
        {
            _db.Materials.Update(material);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Material material)
        {
            _db.Materials.Remove(material);
            await _db.SaveChangesAsync();
        }
    }
}
