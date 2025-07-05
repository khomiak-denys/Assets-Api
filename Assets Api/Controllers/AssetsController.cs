using Assets_Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.SymbolStore;

namespace Assets_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssetsController : ControllerBase {
        private readonly ILogger<AssetsController> _logger;
        private readonly AssetsService _assetsService;

        public AssetsController(ILogger<AssetsController> logger, AssetsService assetsService)
        {
            _logger = logger;
            _assetsService = assetsService;
        }

        [HttpGet(Name = "GetAssets")]
        public async Task<IActionResult> Get() {
            _logger.LogInformation("Fetching assets");
            
            var assets = await _assetsService.GetAssetsAsync();

            return Ok(assets);
        }
    }
}
