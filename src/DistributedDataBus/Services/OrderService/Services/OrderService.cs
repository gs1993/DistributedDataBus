using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static OrderProtoService;

namespace OrderService.Services
{
    public class OrderService : OrderProtoServiceBase
    {


        public override Task<OrderDetails> Get(GetOrderRequest request, ServerCallContext context)
        {
            throw new Exception();
        }

        public override Task<OrderCountResult> GetCount(Empty request, ServerCallContext context)
        {
            throw new Exception();
        }
    }
}
