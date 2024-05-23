using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DocumentManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CsrfTokenController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public CsrfTokenController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        [HttpGet]
        public IActionResult GetCsrfToken()
        {
            try
            {
                var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

                if (tokens.CookieToken != null)
                {
                    HttpContext.Response.Cookies.Append(
                        "XSRF-TOKEN", tokens.CookieToken,
                        new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.None, Secure = true });
                }

                return Ok(new { csrfToken = tokens.RequestToken });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting CSRF token.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting CSRF token.");
            }
        }
    }
}
