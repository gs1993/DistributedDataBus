using Microsoft.AspNetCore.Mvc;

namespace Gateway.Utils
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class GatewayController : Controller
    {
        protected IActionResult ValidationIdError()
        {
            return BadRequest("Invalid id");
        }

    }
}
