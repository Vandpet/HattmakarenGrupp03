using HattmakarenWebbAppGrupp03.Models;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Data.Repositories
{
    public class HatOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly OrderRepository _orderRepository;

        public HatOrderRepository(ApplicationDbContext db, OrderRepository orderRepository)
        {
            _db = db;
            _orderRepository = orderRepository;
        }

        public async Task AddAsync(HatOrder hatOrder)
        {
            _db.HatOrders.Add(hatOrder);
            await _db.SaveChangesAsync();
        }

        public async Task<HatOrder?> GetByIdAsync(int HId, int OId)
        {
            return await _db.HatOrders
                .FirstOrDefaultAsync(ho => ho.HId == HId && ho.OId == OId);
        }

        public async Task<List<HatOrder>> GetAllAsync()
        {
            return await _db.HatOrders
               .ToListAsync();
        }

        public async Task UpdateAsync(HatOrder hatOrder)
        {
            _db.HatOrders.Update(hatOrder);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(HatOrder hatOrder)
        {
            _db.HatOrders.Remove(hatOrder);
            await _db.SaveChangesAsync();
        }

        //Specialmetoder
        public async Task AssignEmployee(HatOrder hatOrder, int EId)
        {
            hatOrder.EId = EId;
            //Sätt status till "Assigned" när en anställd tilldelas eller något liknande
            await _db.SaveChangesAsync();
        }

        public async Task SetPriceOnOrderAsync(int OId)
        {
            var hatOrders = await _db.HatOrders
                .Where(ho => ho.OId == OId)
                .Include(ho => ho.Hat)
                .ToListAsync();

            decimal totalPrice = hatOrders.Sum(ho => ho.Hat.Price * ho.Amount);

            var order = await _db.Orders.FindAsync(OId);

            totalPrice += order.DeliveryFee; // Lägg på fraktkostnad efter expressen

            totalPrice *= 1.25m; // Lägg på moms

            var discountSum = (totalPrice * order.Discount / 100);//Expressen ska inte vara rabatterad.

            if (order.Express) totalPrice *= 1.2m; // Lägg på 20% för expressorder

            totalPrice -= discountSum; // Dra av rabatt

            order.Price = totalPrice;
            await _db.SaveChangesAsync();
        }
        public async Task<decimal> GetPriceWithoutVatAsync(int OId)
        {
            //Hämtar ordern utan express
            var order = await _db.Orders.FindAsync(OId);
            return (order.Price / 1.25m); // Ta bort moms
        }
        public async Task<List<HatOrder>> GetByOrderIdAsync(int OId)
        {
            return await _db.HatOrders
                .Where(ho => ho.OId == OId)
                .Include(ho => ho.Hat)
                .ToListAsync();
        }
        public async Task AddManyAsync(List<HatOrder> hatOrders)
        {
            _db.HatOrders.AddRange(hatOrders);
            await _db.SaveChangesAsync();
        }

        // Dessa metoder används i kalendern
        public async Task ChangeToNotStartedAsync(HatOrder hatOrder)
        {
            hatOrder.Status = "Not Started";
            hatOrder.Date = null;
            hatOrder.EId = null;
            await _db.SaveChangesAsync();

            var order = await _orderRepository.GetByIdAsync(hatOrder.OId);

            // Om en hatorder på ordern inte är Completed längre blir order.status automatiskt inte Completed längre.
            if (order.Status != "Started") order.Status = "Started";

            // Kolla om alla hattar i ordern är Not Started
            var hatOrders = await GetByOrderIdAsync(hatOrder.OId);
            if (hatOrders.All(ho => ho.Status == "Not Started")) order.Status = "Not Started";

            await _db.SaveChangesAsync();
        }
        public async Task ChangeToStartedAsync(HatOrder hatOrder)
        {
            hatOrder.Status = "Started";
            await _db.SaveChangesAsync();
            var order = await _orderRepository.GetByIdAsync(hatOrder.OId);
            order.Status = "Started";
            await _db.SaveChangesAsync();
        }
        public async Task ChangeToCompletedAsync(HatOrder hatOrder)
        {
            hatOrder.Status = "Completed";
            await _db.SaveChangesAsync();

            var order = await _orderRepository.GetByIdAsync(hatOrder.OId);

            // Kolla om alla hattar i ordern är Completed
            var hatOrders = await GetByOrderIdAsync(hatOrder.OId);
            if (hatOrders.All(ho => ho.Status == "Completed"))
            {
                order.Status = "Completed";
                await _db.SaveChangesAsync();
            }
        }
        public async Task ChangeToReturnedAsync(HatOrder hatOrder)
        {
            hatOrder.Status = "Returned";
            await _db.SaveChangesAsync();

            var order = await _orderRepository.GetByIdAsync(hatOrder.OId);

            var hatOrders = await GetByOrderIdAsync(hatOrder.OId);
            // Om alla hatordrar på ordern är Returned blir order.status automatiskt Returned.
            if (hatOrders.All(ho => ho.Status == "Returned")) order.Status = "Fully Returned";
            else order.Status = "Partly Returned"; // Om inte alla är Returned men en är det så är ordern Partly Returned.

            await _db.SaveChangesAsync();
        }
    }
}
