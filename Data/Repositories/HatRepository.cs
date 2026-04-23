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
                 .ThenInclude(hm => hm.Material)
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
        //SpecialMetod för KN_Number och KN_Description
        public List<string> GetKNStringList()
        {
            return new List<string>()
            {
                "6501 00 00 - Filtämnen och oformade hattstumpar",
                "6502 00 00 - Flätade hattämnen av band/remsor",
                "6504 00 00 - Färdiga flätade hattar och huvudbonader",
                "6505 00 10 - Trikå- och textilhattar (ej band)",
                "6505 00 30 - Hårnät av alla material",
                "6505 00 90 - Övriga hattar och huvudbonader",
                "6506 10 10 - Skyddshjälmar av plast (säkerhet)",
                "6506 10 80 - Skyddshjälmar av andra material",
                "6506 91 00 - Gummi- eller plast-huvudbonader ej hjälm",
                "6506 99 10 - Hattämnen av filt från hattstumpar",
                "6506 99 90 - Övriga huvudbonader ej specificerade",
                "6507 00 00 - Hattdelar: svettband, skärmar, remmar"
            };
        }
    }
}
