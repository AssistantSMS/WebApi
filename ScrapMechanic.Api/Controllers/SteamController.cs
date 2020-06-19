﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScrapMechanic.Domain.Contract;
using ScrapMechanic.Domain.Dto;
using ScrapMechanic.Domain.Mapper;
using ScrapMechanic.Integration.Repository.Interface;

namespace ScrapMechanic.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SteamController : ControllerBase
    {
        private readonly IStreamNewsScrapeRepository _newsRepo;

        public SteamController(IStreamNewsScrapeRepository newsRepo)
        {
            _newsRepo = newsRepo;
        }

        /// <summary>
        /// Get News from https://store.steampowered.com/news/?appids=387990.
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        //[CacheFilter(CacheType.HelloGamesRelease)]
        public async Task<ActionResult<List<SteamNewsItemViewModel>>> News()
        {
            List<SteamNewsItem> scrapMechanicNewsItems = await _newsRepo.GetNewsItems("387990");
            return Ok(scrapMechanicNewsItems.ToViewModel());
        }
    }
}
