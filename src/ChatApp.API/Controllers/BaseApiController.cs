using Asp.Versioning;
using ChatApp.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
