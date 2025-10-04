using Core.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected ActionResult HandleResponse<T>(ApiResponse<T> response)
        {
            if (response == null)
            {
                return NotFound(ApiResponse<T>.Fail("Resource not found"));
            }

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
