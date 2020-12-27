using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScrapMechanic.Api.Filter;
using ScrapMechanic.Domain.Contract;
using ScrapMechanic.Domain.Dto.Enum;
using ScrapMechanic.Domain.Dto.ViewModel;
using ScrapMechanic.Domain.Mapper;
using ScrapMechanic.Integration.Service.Interface;

namespace ScrapMechanic.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SteamController : ControllerBase
    {
        private readonly ISteamNewsService _steamNewsService;

        public SteamController(ISteamNewsService steamNewsService)
        {
            _steamNewsService = steamNewsService;
        }

        /// <summary>
        /// Get News from https://store.steampowered.com/news/?appids=387990.
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        [CacheFilter(CacheType.SteamNews, numMinutes:30)]
        public async Task<ActionResult<List<SteamNewsItemViewModel>>> News()
        {
            List<SteamNewsItem> scrapMechanicNewsItems = await _steamNewsService.GetNewsItems("387990");
            if (scrapMechanicNewsItems.Count == 0) return StatusCode(502);
            return Ok(scrapMechanicNewsItems.ToViewModel());
        }
    }
}
