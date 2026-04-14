using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class CustomerManagerRepository
    {
        private readonly ApplicationDbContext _db;

        public CustomerManagerRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(CustomerManager customerManager)
        {
            _db.CustomerManagers.Add(customerManager);
            await _db.SaveChangesAsync();
        }

        public async Task<CustomerManager?> GetByIdAsync(int EId , int CId)
        {
            return await _db.CustomerManagers
                .FirstOrDefaultAsync(cm => cm.EId == EId && cm.CId == CId);
        }

        public async Task<List<CustomerManager>> GetAllAsync()
        {
            return await _db.CustomerManagers
               .ToListAsync();
        }

        public async Task UpdateAsync(CustomerManager customerManager)
        {
            _db.CustomerManagers.Update(customerManager);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(CustomerManager customerManager)
        {
            _db.CustomerManagers.Remove(customerManager);
            await _db.SaveChangesAsync();
        }
    }
}
