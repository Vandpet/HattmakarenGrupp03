using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class HatRepository
    {
        private readonly ApplicationDbContext _db;

        public HatRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Hat hat)
        {
            _db.Hats.Add(hat);
            await _db.SaveChangesAsync();
        }

        public async Task<Hat?> GetByIdAsync(int id)
        {
            return await _db.Hats
                .Include(h => h.Materials)
                .FirstOrDefaultAsync(h => h.HId == id);
        }

        public async Task<List<Hat>> GetAllAsync()
        {
            return await _db.Hats
                .Include(h => h.Materials)
                .ToListAsync();
        }

        public async Task UpdateAsync(Hat hat)
        {
            _db.Hats.Update(hat);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Hat hat)
        {
            _db.Hats.Remove(hat);
            await _db.SaveChangesAsync();
        }
    }
}
