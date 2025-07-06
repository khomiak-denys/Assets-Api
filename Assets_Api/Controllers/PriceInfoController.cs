using Assets_Api.Services;
using Microsoft.AspNetCore.Mvc;

using System;

namespace Assets_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PriceInfoController : ControllerBase {
        private readonly ILogger<PriceInfoController> _logger;
        private readonly PriceInfoService _priceInfoService;
        private readonly PricesStreamingService _streamingService;

        public PriceInfoController(ILogger<PriceInfoController> logger, PriceInfoService priceInfoService, PricesStreamingService streamingService)
        {
            _logger = logger;
            _priceInfoService = priceInfoService;
            _streamingService = streamingService;
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

        [HttpPost("stream")]
        public async Task<IActionResult> StartStreaming([FromBody] List<string> symbols)
        {
            if (symbols == null || !symbols.Any())
            {
                _logger.LogWarning("Symbols parameter is missing or empty.");
                return BadRequest(new { error = "At least one symbol is required." });
            }

            _logger.LogInformation($"Starting streaming for symbols: {string.Join(", ", symbols)}");
            await _streamingService.StartStreamingAsync(symbols, (symbol, price, timestamp) =>
            {
                _logger.LogInformation($"Streamed price: {symbol} = {price:F2} at {timestamp}");
            });

            return Ok(new { message = "Streaming started" });
        }

        [HttpPost("stop-stream")]
        public async Task<IActionResult> StopStreaming()
        {
            await _streamingService.StopStreamingAsync();
            return Ok(new { message = "Streaming stopped" });
        }
    }
}
