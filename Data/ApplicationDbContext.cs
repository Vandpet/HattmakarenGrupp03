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
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Hat> Hats { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialOrder> MaterialOrders { get; set; }
        public DbSet<CustomerManager> CustomerManagers { get; set; }
        public DbSet<AssignedOrders> AssignedOrders { get; set; }

        // Lägg till DbSet för kopplingstabellen så EF hittar den ordentligt
        public DbSet<HatMaterial> HatMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Employee inställningar
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Username)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasQueryFilter(e => !e.IsDeleted);

            // 2. NY RELATION: Hat <-> Material via HatMaterial
            modelBuilder.Entity<HatMaterial>()
                .HasKey(hm => new { hm.HatId, hm.MaterialId });

            modelBuilder.Entity<HatMaterial>()
                .HasOne(hm => hm.Hat)
                .WithMany(h => h.Materials)
                .HasForeignKey(hm => hm.HatId);

            modelBuilder.Entity<HatMaterial>()
                .HasOne(hm => hm.Material)
                .WithMany(m => m.MaterialsForHats)
                .HasForeignKey(hm => hm.MaterialId);

            // 3. Övriga relationer
            modelBuilder.Entity<MaterialOrder>()
                .HasMany(mo => mo.Materials)
                .WithMany(m => m.MaterialOrders);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            // 4. Precision (Decimal-inställningar)
            modelBuilder.Entity<Order>().Property(o => o.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.Discount).HasPrecision(18, 2);

            modelBuilder.Entity<Hat>().Property(h => h.Price).HasPrecision(18, 2);

            modelBuilder.Entity<Material>().Property(m => m.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Material>().Property(m => m.Amount).HasPrecision(18, 2);
        }
    }
}