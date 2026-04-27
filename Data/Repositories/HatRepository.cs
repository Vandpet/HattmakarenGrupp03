using HattmakarenWebbAppGrupp03.Models;
using iText.Commons.Utils;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.IO;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                "6501 00 00 - Felt hat bodies and unshaped hat forms.",
                "6502 00 00 - Braided hat bodies of strips / bands.",
                "6504 00 00 - Finished braided hats and headgear.",
                "6505 00 10 - Knitted / textile hats(not strip - based).",
                "6505 00 30 - Hair nets of all materials.",
                "6505 00 90 - Other hats and headgear.",
                "6506 10 10 - Safety helmets of plastic.",
                "6506 10 80 - Safety helmets of other materials.",
                "6506 91 00 - Rubber or plastic headgear (not helmets).",
                "6506 99 10 - Felt hat bodies made from hat forms.",
                "6506 99 90 - Other headgear, not specified elsewhere.",
                "6507 00 00 - Hat parts: sweatbands, visors, straps."
            };
        }
    }
}
