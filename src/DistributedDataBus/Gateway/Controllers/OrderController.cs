using Contracts.Orders.Models;
using DataBus.Requests;
using Gateway.Utils;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Gateway.Controllers
{
    public class OrderController : GatewayController
    {
        private readonly OrderProtoService.OrderProtoServiceClient _orderProtoServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderController(OrderProtoService.OrderProtoServiceClient orderProtoServiceClient, IPublishEndpoint publishEndpoint)
        {
            _orderProtoServiceClient = orderProtoServiceClient;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(OrderDetailsDto))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id < 1)
                return ValidationIdError();

            var order = await _orderProtoServiceClient
                .GetAsync(new GetOrderRequest { OrderId = id }, cancellationToken: cancellationToken)
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
        public async Task<IActionResult> Count(CancellationToken cancellationToken)
        {
            var countResult = await _orderProtoServiceClient
                .GetCountAsync(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return countResult != null
                ? Ok(countResult.Count)
                : NotFound();
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.Accepted)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest();

            await _publishEndpoint.Publish(new CreateOrderRequest
            {
                Name = request.Name
            }, cancellationToken).ConfigureAwait(false);

            return Accepted();
        }
    }
}
