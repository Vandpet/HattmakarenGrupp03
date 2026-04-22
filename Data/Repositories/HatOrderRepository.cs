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

            var discountSum = (totalPrice * order.Discount / 100);//Expressen ska inte vara rabatterad.

            if (order.Express) totalPrice *= 1.2m; // Lägg på 20% för expressorder

            totalPrice -= discountSum; // Dra av  rabatt

            order.Price = totalPrice;
            await _db.SaveChangesAsync();
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
            hatOrder.Status = "Ej Påbörjad";
            hatOrder.Date = null;
            hatOrder.EId = null;
            await _db.SaveChangesAsync();

            var order = await _orderRepository.GetByIdAsync(hatOrder.OId);

            // Om en hatorder på ordern inte är färdig längre blir order.status automatiskt inte färdig längre.
            if (order.Status != "Påbörjad") order.Status = "Påbörjad";

            // Kolla om alla hattar i ordern är Ej Påbörjade
            var hatOrders = await GetByOrderIdAsync(hatOrder.OId);
            if (hatOrders.All(ho => ho.Status == "Ej Påbörjad")) order.Status = "Ej Påbörjad";

            await _db.SaveChangesAsync();
        }
        public async Task ChangeToStartedAsync(HatOrder hatOrder)
        {
            hatOrder.Status = "Påbörjad";
            await _db.SaveChangesAsync();
            var order = await _orderRepository.GetByIdAsync(hatOrder.OId);
            order.Status = "Påbörjad";
            await _db.SaveChangesAsync();
        }
        public async Task ChangeToCompletedAsync(HatOrder hatOrder)
        {
            hatOrder.Status = "Färdig";
            await _db.SaveChangesAsync();

            var order = await _orderRepository.GetByIdAsync(hatOrder.OId);

            // Kolla om alla hattar i ordern är färdiga
            var hatOrders = await GetByOrderIdAsync(hatOrder.OId);
            if (hatOrders.All(ho => ho.Status == "Färdig"))
            {
                order.Status = "Färdig";
                await _db.SaveChangesAsync();
            }
        }
        public async Task ChangeToReturnedAsync(HatOrder hatOrder)
        {
            hatOrder.Status = "Returnerad";
            await _db.SaveChangesAsync();

            var order = await _orderRepository.GetByIdAsync(hatOrder.OId);

            var hatOrders = await GetByOrderIdAsync(hatOrder.OId);
            // Om alla hatordrar på ordern är returnerad blir order.status automatiskt returnerad.
            if (hatOrders.All(ho => ho.Status == "Returnerad")) order.Status = "Helt Returnerad";
            else order.Status = "Delvis Returnerad"; // Om inte alla är returnerade men en är det så är ordern delvis returnerad.

            await _db.SaveChangesAsync();
        }
    }
}
