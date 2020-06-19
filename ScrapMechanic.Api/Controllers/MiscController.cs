using Microsoft.AspNetCore.Mvc;

namespace ScrapMechanic.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MiscController : ControllerBase
    {
        /// <summary>
        /// Handle HEAD requests to root of API.
        /// </summary>
        [HttpHead]
        [Route("/")]
        [Route("/{*url}", Order = 998)]
        public IActionResult Head()
        {
            return Ok("API running...");
        }

        /// <summary>
        /// Handle requests for favicon
        /// </summary>
        [HttpGet]
        [Route("/favicon.ico")]
        public IActionResult HandleFaviconRequest()
        {
            return RedirectPermanent("https://scrapassistant.com/favicon.ico");
        }

        /// <summary>
        /// Handle requests for robots.txt
        /// </summary>
        [HttpGet]
        [Route("/robots.txt")]
        public IActionResult HandleRobots()
        {
            return RedirectPermanent("https://scrapassistant.com/robots.txt");
        }

        /// <summary>
        /// Handle not found requests. Version requests redirected to /version. Everything else redirected to Swagger Docs
        /// </summary>
        [HttpGet]
        [Route("/{*url}", Order = 999)]
        public IActionResult CatchAll(string url)
        {
            return RedirectPermanent("https://api.scrapassistant.com");
        }
    }
}