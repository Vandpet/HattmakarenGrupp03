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

        public DbSet<Email> Email { get; set; }


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

            modelBuilder.Entity<Order>()
                .HasOne(o => o.StartedBy)
                .WithMany()
                .HasForeignKey(o => o.StartedById);
                
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

            var material1 = new Material
            {
                Name = "Hundläder",
                Amount = 100,
                MeasuringUnits = "meter",
                Price = 50m
            };

            var material2 = new Material
            {
                Name = "Spökmaterial",
                Amount = 100,
                MeasuringUnits = "meter",
                Price = 50m
            };

            var material3 = new Material
            {
                Name = "Läder",
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
                KN_Description = "Felt hat bodies and unshaped hat forms."
            };

            var hatMaterial = new HatMaterial
            {
                Hat = hat,
                Material = material
            };

            var hat1 = new Hat
            {
                Name = "Hundhatt",
                Price = 500m,
                Size = "petit",
                StandardHat = false,
                PicturePath = "/uploads/a34ba6d8-a3c3-4c43-afeb-c493eae1b0df.webp",
                Description = "Hundhatt, specialbeställd för hundar",
                KN_Number = "6504 00 00",
                KN_Description = "Felt hat bodies and unshaped hat forms."
            };

            var hat2 = new Hat
            {
                Name = "Hatt utan bild",
                Price = 500m,
                Size = "vet ej",
                StandardHat = false,
                PicturePath = "",
                Description = "En hatt utan bild",
                KN_Number = "6504 00 00",
                KN_Description = "Felt hat bodies and unshaped hat forms."
            };

            var hat3 = new Hat
            {
                Name = "CowboyHatt",
                Price = 500m,
                Size = "pyttelitet",
                StandardHat = true,
                PicturePath = "/uploads/5138f31d-9454-4155-af25-a95c4f004557.jpg",
                Description = "Hatt för en groda. Lorem ipsum dolor sit amet.",
                KN_Number = "6504 00 00",
                KN_Description = "Felt hat bodies and unshaped hat forms."
            };

            var hatMaterial1 = new HatMaterial
            {
                Hat = hat1,
                Material = material1
            };

            var hatMaterial2 = new HatMaterial
            {
                Hat = hat2,
                Material = material2
            };

            var hatMaterial3 = new HatMaterial
            {
                Hat = hat3,
                Material = material3
            };

            // --- Order ---
            var order = new Order
            {
                Status = "Not Started",
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
                Status = "Started",
                Express = false,
                Discount = 0,
                DiscountDesc = "",
                OrderDate = DateTime.Now.AddDays(-2),
                PrelDeliveryDate = DateTime.Now.AddDays(5),
                Description = "Order Started",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 50
            };

            var order2 = new Order
            {
                Status = "Completed",
                Express = false,
                Discount = 0,
                DiscountDesc = "",
                OrderDate = DateTime.Now.AddDays(-5),
                PrelDeliveryDate = DateTime.Now.AddDays(2),
                Description = "Order Completed",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 100
            };

            var order3 = new Order
            {
                Status = "Shipped",
                Express = true,
                Discount = 10,
                DiscountDesc = "Test rabatt",
                OrderDate = DateTime.Now.AddDays(-10),
                PrelDeliveryDate = DateTime.Now.AddDays(-1),
                Description = "Order Shipped",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 150
            };

            var order4 = new Order
            {
                Status = "Shipped",
                Express = false,
                Discount = 0,
                DiscountDesc = "Test rabatt",
                OrderDate = DateTime.Now.AddDays(-100),
                PrelDeliveryDate = DateTime.Now.AddDays(-1),
                Description = "Order Shipped",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 150,
                Price = 350
            };

            var order5 = new Order
            {
                Status = "Shipped",
                Express = false,
                Discount = 0,
                DiscountDesc = "Test rabatt",
                OrderDate = DateTime.Now.AddDays(-30),
                PrelDeliveryDate = DateTime.Now.AddDays(-1),
                Description = "Order Shipped",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 150,
                Price = 350
            };

            var order6 = new Order
            {
                Status = "Shipped",
                Express = false,
                Discount = 0,
                DiscountDesc = "Test rabatt",
                OrderDate = DateTime.Now.AddDays(-400),
                PrelDeliveryDate = DateTime.Now.AddDays(-1),
                Description = "Order Shipped",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 150,
                Price = 350
            };

            var order7 = new Order
            {
                Status = "Shipped",
                Express = false,
                Discount = 0,
                DiscountDesc = "Test rabatt",
                OrderDate = DateTime.Now.AddDays(-200),
                PrelDeliveryDate = DateTime.Now.AddDays(-1),
                Description = "Order Shipped",
                Customer = customer,
                CreatedBy = otto,
                DeliveryFee = 150,
                Price = 350
            };

            var hatorder = new HatOrder
            {
                Hat = hat,
                Order = order,
                Employee = otto,
                Status = "Not Started",
                Date = DateTime.Now.AddDays(0),
                Amount = 1
            };

            var hatOrder1 = new HatOrder
            {
                Hat = hat,
                Order = order1,
                Employee = otto,
                Status = "Started",
                Date = DateTime.Now.AddDays(0),
                Amount = 1
            };

            var hatOrder2 = new HatOrder
            {
                Hat = hat,
                Order = order2,
                Employee = otto,
                Status = "Completed",
                Date = DateTime.Now.AddDays(1),
                Amount = 2
            };

            var hatOrder3 = new HatOrder
            {
                Hat = hat,
                Order = order3,
                Employee = otto,
                Status = "Shipped",
                Date = DateTime.Now.AddDays(2),
                Amount = 1
            };

            var hatOrder4 = new HatOrder
            {
                Hat = hat,
                Order = order4,
                Employee = otto,
                Status = "Shipped",
                Date = DateTime.Now.AddDays(-100),
                Amount = 1
            };

            var hatOrder5 = new HatOrder
            {
                Hat = hat2,
                Order = order5,
                Employee = otto,
                Status = "Shipped",
                Date = DateTime.Now.AddDays(-30),
                Amount = 1
            };

            var hatOrder6 = new HatOrder
            {
                Hat = hat2,
                Order = order6,
                Employee = otto,
                Status = "Shipped",
                Date = DateTime.Now.AddDays(-400),
                Amount = 1
            };

            var hatOrder7 = new HatOrder
            {
                Hat = hat3,
                Order = order7,
                Employee = otto,
                Status = "Shipped",
                Date = DateTime.Now.AddDays(-200),
                Amount = 1
            };

                


            // --- Add everything ---
            context.AddRange(
                otto,
                felicia,
                customer,
                material,
                hat,
                hat1,
                hat2,
                hat3,
                hatMaterial,
                hatMaterial1,
                hatMaterial2,
                hatMaterial3,
                order,
                order1,
                order2,
                order3,
                order4,
                order5,
                order6,
                order7,
                hatorder,
                hatOrder1,
                hatOrder2,
                hatOrder3,
                hatOrder4,
                hatOrder5,
                hatOrder6,
                hatOrder7
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