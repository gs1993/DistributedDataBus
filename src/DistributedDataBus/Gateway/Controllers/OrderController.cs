using Contracts.Orders.Models;
using Gateway.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Gateway.Controllers
{
    public class OrderController : GatewayController
    {
        private readonly OrderProtoService.OrderProtoServiceClient _orderProtoServiceClient;

        public OrderController(OrderProtoService.OrderProtoServiceClient orderProtoServiceClient)
        {
            _orderProtoServiceClient = orderProtoServiceClient;
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(OrderDetailsDto))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id < 1)
                return ValidationIdError();

            var order = await _orderProtoServiceClient
                .GetAsync(new GetOrderRequest { OrderId = id })
                .ConfigureAwait(false);

            return order != null
                ? Ok(new OrderDetailsDto(order))
                : NotFound();
        }

        [HttpGet]
        [Route("count")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Count()
        {
            var countResult = await _orderProtoServiceClient
                .GetCountAsync(new Google.Protobuf.WellKnownTypes.Empty())
                .ConfigureAwait(false);

            return countResult != null
                ? Ok(countResult.Count)
                : NotFound();
        }
    }
}
