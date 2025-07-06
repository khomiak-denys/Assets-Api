using Assets_Api.Database.Repositiries;
using Assets_Api.Models;
using Assets_Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Assets_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PriceInfoController : ControllerBase {
        private readonly ILogger<PriceInfoController> _logger;
        private readonly PriceInfoService _priceInfoService;
        
        public PriceInfoController(ILogger<PriceInfoController> logger, PriceInfoService priceInfoService)
        {
            _logger = logger;
            _priceInfoService = priceInfoService;

        }

        [HttpGet(Name = "GetPrices")]
        public async Task<IActionResult> Get([FromQuery] string symbol) {
            _logger.LogInformation("Fetching prices");
            var prices = await _priceInfoService.GetPricesAsync(symbol);
            return Ok(prices);
        }
    }
}
