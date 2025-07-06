using Assets_Api.Database.Repositiries;
using Assets_Api.Models;
using Assets_Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Assets_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PriceInfoController : ControllerBase {
        private readonly ILogger<PriceInfoController> _logger;
        private readonly PriceInfoService _priceInfoService;
        
        public PriceInfoController(ILogger<PriceInfoController> logger, PriceInfoService priceInfoService)
        {
            _logger = logger;
            _priceInfoService = priceInfoService;

        }

        [HttpGet(Name = "GetPrices")]
        public async Task<IActionResult> Get([FromQuery(Name = "symbols")] List<string> symbols) {
            _logger.LogInformation("Fetching prices");
            foreach (var symbol in symbols)
            {
                _logger.LogInformation($"Fetching price for symbol: {symbol}");
            }
            var prices = await _priceInfoService.GetPricesAsync(symbols);
            return Ok(prices);
        }
    }
}
