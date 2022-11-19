using Dawn;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> Get(int id, CancellationToken ct = default);
        Task<int> Count(CancellationToken ct = default);
        Task Add(string name, CancellationToken ct = default);
        Task ChangeStatus(int id, string newStatus, CancellationToken ct = default);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _orderDbContext;

        public OrderRepository(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task<Order?> Get(int id, CancellationToken ct)
        {
            return await _orderDbContext
                .Orders
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<int> Count(CancellationToken ct)
        {
            return await _orderDbContext
                .Orders
                .CountAsync(ct);
        }

        public async Task Add(string name, CancellationToken ct)
        {
            Guard.Argument(name)
                .NotWhiteSpace()
                .LengthInRange(5, 150);

            _orderDbContext.Attach(new Order(0, name, "Created"));
            await _orderDbContext.SaveChangesAsync(ct);
        }

        public async Task ChangeStatus(int id, string newStatus, CancellationToken ct)
        {
            Guard.Argument(id)
                .Positive();
            Guard.Argument(newStatus)
                .NotWhiteSpace()
                .LengthInRange(5, 150);

            var order = await _orderDbContext
                .Orders
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                throw new Exception("Order does not exist");

            order.Status = newStatus;
            await _orderDbContext.SaveChangesAsync(ct);
        }
    }
}
