using DataBus.Requests.Order;
using MassTransit;
using OrderService.Repositories;

namespace OrderService.Consumers
{
    public class CancelOrderConsumer : IConsumer<CancelOrderRequest>
    {
        private readonly IOrderRepository _orderRepository;

        public CancelOrderConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<CancelOrderRequest> context)
        {
            int orderId = context.Message.Id;
            var order = await _orderRepository.Get(orderId);
            if (order == null)
                throw new InvalidOperationException($"Cannot find order with id: {orderId}");
            if (order.Status != "Created")
                throw new InvalidOperationException($"Order with id: {orderId} has invalid state: {order.Status}");

            await _orderRepository.ChangeStatus(orderId, "Cancelled");
        }
    }
}
