using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class EmployeeRepository
    {
        private readonly ApplicationDbContext _db;
        public EmployeeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Employee employee)
        {
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
        }
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _db.Employees
                .FirstOrDefaultAsync(e => e.EId == id);
        }
        public async Task<List<Employee>> GetAllAsync()
        {
            return await _db.Employees
                .ToListAsync();
        }
        public async Task UpdateAsync(Employee employee)
        {
            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Employee employee)
        {
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
        }
        //Specialmetoder
        //Denna kan användas för Kalendern senare
        //public async Task<Employee?> GetByIdWithAssignedOrders(int id)
        //{
        //    return await _db.Employees
        //        .Include(e => e.TakenOrders)
        //        .FirstOrDefaultAsync(e => e.EId == id);
        //}
    }
}
