using System.Net;
using System.Text.Json;

namespace Gateway.Utils
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                string errorMessage;

                switch (error)
                {
                    //TODO: add app exception
                    case KeyNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        errorMessage = error.Message;
                        break;
                    default:
                        _logger.LogError(error, error.Message);
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorMessage = "Internal server error";
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = errorMessage });
                await response.WriteAsync(result);
            }
        }
    }
}
