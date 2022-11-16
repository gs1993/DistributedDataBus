using DataBus.Requests;
using MassTransit;

namespace OrderService.Consumers
{
    public class CreateOrderConsumer : IConsumer<CreateOrderRequest>
    {
        public Task Consume(ConsumeContext<CreateOrderRequest> context)
        {


            return Task.CompletedTask;
        }
    }
}
