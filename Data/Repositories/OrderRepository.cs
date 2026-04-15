using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;


namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class OrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _db.Orders
                .FirstOrDefaultAsync(o => o.OId == id);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _db.Orders
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

        //Specialmetoder
        //Denna kan användas för Kalendern senare
        //public async Task<Order?> GetByIdWithAssignedEmployees(int id)
        //{
        //    return await _db.Orders
        //        .Include(o => o.AssignedEmployees)
        //        .FirstOrDefaultAsync(o => o.OId == id);
        //}
    }
}
