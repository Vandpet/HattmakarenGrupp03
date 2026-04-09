using Microsoft.EntityFrameworkCore;
using HattmakarenWebbAppGrupp03.Models; 

namespace HattmakarenWebbAppGrupp03.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

    }
}