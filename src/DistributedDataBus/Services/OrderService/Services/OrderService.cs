using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OrderService.Repositories;
using static OrderProtoService;

namespace OrderService.Services
{
    public class OrderService : OrderProtoServiceBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public override async Task<OrderDetails?> Get(GetOrderRequest request, ServerCallContext context)
        {
            var order = await _orderRepository.Get(request.OrderId, context.CancellationToken);

            return order != null
               ? new OrderDetails
               {
                   Id = order.Id,
                   Name = order.Name,
                   Status = order.Status
               } : null;
        }

        public override async Task<OrderCountResult> GetCount(Empty request, ServerCallContext context)
        {
            int count = await _orderRepository.Count(context.CancellationToken);
            return new OrderCountResult
            {
                Count = count
            };
        }
    }
}
