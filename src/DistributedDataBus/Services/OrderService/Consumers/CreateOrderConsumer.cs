using DataBus.Requests;
using MassTransit;
using OrderService.Repositories;

namespace OrderService.Consumers
{
    public class CreateOrderConsumer : IConsumer<CreateOrderRequest>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<CreateOrderRequest> context)
        {
            await _orderRepository.Add(context.Message.Name);
        }
    }
}
