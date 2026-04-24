using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class CustomerRepository
    {
        private readonly ApplicationDbContext _db;
        public CustomerRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Customer customer)
        {
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
        }
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _db.Customers
                .FirstOrDefaultAsync(c => c.CId == id);
        }
        public async Task<List<Customer>> GetAllAsync()
        {
            return await _db.Customers
                .ToListAsync();
        }
        public async Task UpdateAsync(Customer customer)
        {
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }

        // Specialmetoder
        // Om kund är från utlandet
        public async Task<bool> IsForeignCustomerAsync(int CId)
        {
            var customer = await GetByIdAsync(CId);
            if (!customer.Country.ToLower().Trim().Equals("sweden") &&
                !customer.Country.ToLower().Trim().Equals("sverige"))
            {
                return true;
            }
            return false;
        }

    }
}
