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

        // Tabeller baserade på dina entiteter i diagrammet
        public DbSet<Employee> Employees { get; set; }
        
        public DbSet<Customer> Customers { get; set; }
       
        public DbSet<Order> Orders { get; set; }
       
        public DbSet<Hat> Hats { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialOrder> MaterialOrders { get; set; }
        public DbSet<CustomerManager> CustomerManagers { get; set; }

        public DbSet<AssignedOrders> AssignedOrders { get; set; }
        public DbSet<OrderOfMaterials> OrderOfMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Username)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Hat>()
                .HasMany(h => h.Materials)
                .WithMany(m => m.Hats);

            //modelBuilder.Entity<MaterialOrder>()
            //    .HasMany(mo => mo.Materials)
            //    .WithMany(m => m.MaterialOrders);

            modelBuilder.Entity<Order>().Property(o => o.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Hat>().Property(h => h.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Material>().Property(m => m.Price).HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);
            
            modelBuilder.Entity<Material>()
                .Property(m => m.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Discount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Hat>()
                .Property(h => h.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Material>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Price)
                .HasPrecision(18, 2);

            //CustomerManager
            modelBuilder.Entity<CustomerManager>()
                .HasKey(ec => new { ec.EId, ec.CId });

            modelBuilder.Entity<CustomerManager>()
                .HasOne(ec => ec.Employee)
                .WithMany(e => e.ManagedCustomers)
                .HasForeignKey(ec => ec.EId);

            modelBuilder.Entity<CustomerManager>()
                .HasOne(ec => ec.Customer)
                .WithMany(c => c.Managed)
                .HasForeignKey(ec => ec.CId);

            //AssignedOrders
            modelBuilder.Entity<AssignedOrders>()
                .HasKey(ao => new { ao.EId, ao.OId });

            modelBuilder.Entity<AssignedOrders>()
                .HasOne(ao => ao.Employee)
                .WithMany(e => e.TakenOrders)
                .HasForeignKey(ao => ao.EId);

            modelBuilder.Entity<AssignedOrders>()
                .HasOne(ao => ao.Order)
                .WithMany(o => o.AssignedEmployees)
                .HasForeignKey(ao => ao.OId);

            //OrderOfMaterials
            modelBuilder.Entity<OrderOfMaterials>()
                .HasKey(om => new { om.OId, om.MoId });

            modelBuilder.Entity<OrderOfMaterials>()
                .HasOne(om => om.Order)
                .WithMany(o => o.MaterialOrders)
                .HasForeignKey(om => om.OId);

            modelBuilder.Entity<OrderOfMaterials>()
                .HasOne(om => om.MaterialOrder)
                .WithMany(mo => mo.Orders)
                .HasForeignKey(om => om.MoId);
        }
    }
}