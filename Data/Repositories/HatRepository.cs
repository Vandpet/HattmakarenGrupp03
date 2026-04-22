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
        //SpecialMetod för KN_Number och KN_Description
        public List<string> GetKNStringList()
        {
            return new List<string>()
            {
                "6501 00 00 - filtämnen och oformade hattstumpar",
                "6502 00 00 - flätade hattämnen av band/remsor",
                "6504 00 00 - färdiga flätade hattar och huvudbonader",
                "6505 00 10 - trikå- och textilhattar (ej band)",
                "6505 00 30 - hårnät av alla material",
                "6505 00 90 - övriga hattar och huvudbonader",
                "6506 10 10 - skyddshjälmar av plast (säkerhet)",
                "6506 10 80 - skyddshjälmar av andra material",
                "6506 91 00 - gummi- eller plast-huvudbonader ej hjälm",
                "6506 99 10 - hattämnen av filt från hattstumpar",
                "6506 99 90 - övriga huvudbonader ej specificerade",
                "6507 00 00 - hattdelar: svettband, skärmar, remmar"
            };
        }
    }
}
