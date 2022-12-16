using Gateway.Utils;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using DataBus.Requests.Book;
using Contracts.Book;
using static BookProtoService;

namespace Gateway.Controllers
{
    public class BookController : GatewayController
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly BookProtoServiceClient _bookProtoServiceClient;

        public BookController(IPublishEndpoint publishEndpoint, BookProtoServiceClient bookProtoServiceClient)
        {
            _publishEndpoint = publishEndpoint;
            _bookProtoServiceClient = bookProtoServiceClient;
        }

        [HttpGet]
        [Route("{isbn}")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(BookDetailsDto))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromRoute] string isbn, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(isbn) || isbn.Length != 10)
                return ValidationIdError();

            var bookDto = await _bookProtoServiceClient
                .GetAsync(new GetBookRequest { Isbn = isbn }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return bookDto != null
                ? Ok(bookDto)
                : NotFound();
        }

        [HttpGet]
        [Route("count")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Count(CancellationToken cancellationToken)
        {
            var countResult = await _bookProtoServiceClient
                .GetCountAsync(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return countResult != null
                ? Ok(countResult.Count)
                : NotFound();
        }

        [HttpPost]
        [Route("Import")]
        [SwaggerResponse((int)HttpStatusCode.Accepted)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Import([FromBody] ImportBookDto request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Isbn) || request.Isbn.Length != 10)
                return ValidationIdError();

            await _publishEndpoint.Publish(new ImportBookRequest(request.Isbn), cancellationToken)
                .ConfigureAwait(false);

            return Accepted();
        }

        [HttpPost]
        [Route("BatchImport/{ISBN}")]
        [SwaggerResponse((int)HttpStatusCode.Accepted)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ImportMany([FromBody] ImportBatchBookDto request, CancellationToken cancellationToken)
        {
            if (request is null || request.Isbns == null || !request.Isbns.Any(x => x.Length != 10))
                return ValidationIdError();

            var messages = request.Isbns
                .Select(x => new ImportBookRequest(x));

            await _publishEndpoint.PublishBatch(messages, cancellationToken)
                .ConfigureAwait(false);

            return Accepted();
        }
    }
}
