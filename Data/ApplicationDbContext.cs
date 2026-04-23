using HattmakarenWebbAppGrupp03.Data.Repositories;
using HattmakarenWebbAppGrupp03.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<OrderOfMaterials> OrderOfMaterials { get; set; }
        public DbSet<HatOrder> HatOrders { get; set; }
        public DbSet<HatMaterial> HatMaterials { get; set; }
        public DbSet<CustomActivity> CustomActivities { get; set; }
        public DbSet<HatSchedule> HatSchedule { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee inställningar
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Username)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasQueryFilter(e => !e.IsDeleted);

            // Precision (Decimal-inställningar)
            modelBuilder.Entity<Order>().Property(o => o.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.Discount).HasPrecision(18, 2);

            modelBuilder.Entity<Hat>().Property(h => h.Price).HasPrecision(18, 2);

            modelBuilder.Entity<Material>().Property(m => m.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Material>().Property(m => m.Amount).HasPrecision(18, 2);

            // Övriga relationer
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.CreatedBy)
                .WithMany(e => e.HandledOrders) // Om du inte har en lista i Employee som heter CreatedOrders
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            //ActivityTabell med relation till Employee
            modelBuilder.Entity<CustomActivity>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Activities)
                .HasForeignKey(a => a.EId);

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

            //HatMaterials
            modelBuilder.Entity<HatMaterial>()
                .HasKey(hm => new { hm.HId, hm.MId });

            modelBuilder.Entity<HatMaterial>()
                .HasOne(hm => hm.Hat)
                .WithMany(h => h.Materials)
                .HasForeignKey(hm => hm.HId);

            modelBuilder.Entity<HatMaterial>()
                .HasOne(hm => hm.Material)
                .WithMany(m => m.MaterialsForHats)
                .HasForeignKey(hm => hm.MId);

            //HatOrders
            modelBuilder.Entity<HatOrder>()
                .HasKey(ho => new { ho.HId, ho.OId });

            modelBuilder.Entity<HatOrder>()
                .HasOne(ho => ho.Hat)
                .WithMany(h => h.HatInOrders)
                .HasForeignKey(ho => ho.HId);

            modelBuilder.Entity<HatOrder>()
                .HasOne(ho => ho.Order)
                .WithMany(o => o.HatOrders)
                .HasForeignKey(ho => ho.OId);

            // Måste inte finnas en Employee kopplad till en HatOrder, så den är optional
            modelBuilder.Entity<HatOrder>()
                .HasOne(ho => ho.Employee)
                .WithMany(e => e.AssignedHats)
                .HasForeignKey(ho => ho.EId)
                .IsRequired(false);

            modelBuilder.Entity<ConversationParticipant>()
    .HasKey(cp => new { cp.ConversationId, cp.EmployeeId });

            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.Employee)
                .WithMany()
                .HasForeignKey(cp => cp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(c => c.CreatedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.SenderEmployee)
                .WithMany()
                .HasForeignKey(m => m.SenderEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Schema
            //modelBuilder.Entity<HatSchedule>()
            //   .HasOne(s => s.HatOrder)
            //   .WithMany()
            //   .HasForeignKey(s => s.HatOrderId);
        }

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure DB exists
            await context.Database.EnsureCreatedAsync();

            // Avoid duplicate seeding
            if (context.Employees.Any())
                return;
            var _passwordHasher = new PasswordHasher<Employee>();

            // --- Employee (Otto) ---
            var otto = new Employee
            {
                Name = "Otto",
                Adress = "Testgatan 1",
                PhoneNr = "0700000000",
                Email = "Otto@hatmakarna.se",
                accesslevel = 9,
                Username = "Otto"
            };

            var felicia = new Employee
            {
                Name = "Felicia",
                Adress = "Testgatan 1",
                PhoneNr = "0700000000",
                Email = "Otto@hatmakarna.se",
                accesslevel = 10,
                Username = "Felicia"
            };
            otto.PasswordHash = _passwordHasher.HashPassword(otto, "asdasd");

            felicia.PasswordHash = _passwordHasher.HashPassword(felicia, "123123");



            // --- Customer ---
            var customer = new Customer
            {
                Name = "Test Kund",
                Adress = "Kundgatan 2",
                PhoneNr = "0711111111",
                Country = "Sweden",
                Email = "testkund@example.com",
                City = "Örebro",
                Language = "SV"
            };

            var customer1 = new Customer
            {
                Name = "Italiensk Kund",
                Adress = "Kundgatan 2",
                PhoneNr = "0711111111",
                Country = "Italy",
                Email = "italienskkund@example.com",
                City = "Rome",
                Language = "IT"
            };

            // --- Material ---
            var material = new Material
            {
                Name = "Ull",
                Amount = 100,
                MeasuringUnits = "meter",
                Price = 50m
            };

            // --- Hat (adjust if your model differs) ---
            var hat = new Hat
            {
                Name = "Standardhatt",
                Price = 200m,
                Size = "M",
                StandardHat = true,
                PicturePath = "/uploads/740ccec0-24a4-4e2f-915a-e34cd28a3ff9.jpg",
                Description = "Testhatt",
                KN_Number = "6504 00 00",
                KN_Description = "Färdiga flätade hattar och huvudbonader"
            };

            // --- Order ---
            var order = new Order
            {
                Status = "Ej Påbörjad",
                Express = false,
                Discount = 0,
                DiscountDesc = "",
                OrderDate = DateTime.Now,
                PrelDeliveryDate = DateTime.Now.AddDays(7),
                Description = "Testorder",
                Customer = customer1,
                CreatedBy = otto,
                DeliveryFee = 0
            };



            var order1 = new Order
            {
                Status = "Påbörjad",
                Express = false,
                Discount = 0,
                DiscountDesc = "",
                OrderDate = DateTime.Now.AddDays(-2),
                PrelDeliveryDate = DateTime.Now.AddDays(5),
                Description = "Order påbörjad",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 50
            };

            var order2 = new Order
            {
                Status = "Färdig",
                Express = false,
                Discount = 0,
                DiscountDesc = "",
                OrderDate = DateTime.Now.AddDays(-5),
                PrelDeliveryDate = DateTime.Now.AddDays(2),
                Description = "Order färdig",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 100
            };

            var order3 = new Order
            {
                Status = "Skickad",
                Express = true,
                Discount = 10,
                DiscountDesc = "Test rabatt",
                OrderDate = DateTime.Now.AddDays(-10),
                PrelDeliveryDate = DateTime.Now.AddDays(-1),
                Description = "Order skickad",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 150
            };

            var hatorder = new HatOrder
            {
                Hat = hat,
                Order = order,
                Employee = otto,
                Status = "Ej Påbörjad",
                Date = DateTime.Now.AddDays(0),
                Amount = 1
            };

            var hatOrder1 = new HatOrder
            {
                Hat = hat,
                Order = order1,
                Employee = otto,
                Status = "Påbörjad",
                Date = DateTime.Now.AddDays(0),
                Amount = 1
            };

            var hatOrder2 = new HatOrder
            {
                Hat = hat,
                Order = order2,
                Employee = otto,
                Status = "Färdig",
                Date = DateTime.Now.AddDays(1),
                Amount = 2
            };

            var hatOrder3 = new HatOrder
            {
                Hat = hat,
                Order = order3,
                Employee = otto,
                Status = "Skickad",
                Date = DateTime.Now.AddDays(2),
                Amount = 1
            };


            // --- Add everything ---
            context.AddRange(
                otto,
                felicia,
                customer,
                material,
                hat,
                order,
                order1,
                order2,
                order3,
                hatorder,
                hatOrder1,
                hatOrder2,
                hatOrder3
            );

            await context.SaveChangesAsync();

            HatOrderRepository _hatOrderRepo = new HatOrderRepository(context, new OrderRepository(context));
            await _hatOrderRepo.SetPriceOnOrderAsync(order.OId);
            await _hatOrderRepo.SetPriceOnOrderAsync(order1.OId);
            await _hatOrderRepo.SetPriceOnOrderAsync(order2.OId);
            await _hatOrderRepo.SetPriceOnOrderAsync(order3.OId);
            await context.SaveChangesAsync();
        }
    }
}