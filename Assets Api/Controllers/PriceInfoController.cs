using Assets_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Assets_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PriceInfoController : ControllerBase {
        private readonly ILogger<PriceInfoController> _logger;
        public PriceInfoController(ILogger<PriceInfoController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetPrices")]
        public IEnumerable<PriceInfo> Get() {
            _logger.LogInformation("Fetching prices");
            return Enumerable.Range(1, 5).Select(index => new PriceInfo
            {
                Id = index,
                Symbol = $"Symbol{index}",
                Price = (decimal)(Random.Shared.Next(100, 1000) + Random.Shared.NextDouble()),
                LastUpdate = DateTime.Now.AddMinutes(0)
            })
            .ToArray();
        }

    }
}
