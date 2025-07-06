using Assets_Api.Database.Repositiries;
using Assets_Api.Models;

namespace Assets_Api.Services
{
    public class PriceInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly FintachartsAuthService _authService;
        private readonly AssetsRepository _assetsRepository;
        public PriceInfoService(HttpClient httpClient, FintachartsAuthService authService, AssetsRepository assetsRepository)
        {
            _httpClient = httpClient;
            _authService = authService;
            _assetsRepository = assetsRepository;
        }

        public async Task<Assets> GetPricesAsync(string symbol)
        {
            var result = await _assetsRepository.GetAssetBySymbolAsync(symbol);
            
            return result;
        }
    }
}
