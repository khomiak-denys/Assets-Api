using Assets_Api.Database.Repositiries;
using System.Net.Http.Headers;
using System.Text.Json;
using Assets_Api.Models;

namespace Assets_Api.Services
{
    public class PriceInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly FintachartsAuthService _authService;
        private readonly AssetsRepository _assetsRepository;
        private readonly PriceInfoRepository _priceInfoRepository;

        private readonly IConfiguration _config;    
        public PriceInfoService(HttpClient httpClient, FintachartsAuthService authService, AssetsRepository assetsRepository, IConfiguration config, PriceInfoRepository priceInfoRepository)
        {
            _httpClient = httpClient;
            _authService = authService;
            _assetsRepository = assetsRepository;
            _priceInfoRepository = priceInfoRepository;
            _config = config;
        }

        public async Task<IEnumerable<PriceInfo>> GetPricesAsync(string symbol)
        {
            var result = await _assetsRepository.GetAssetBySymbolAsync(symbol);

            var instrumentId = result.id;
            var provider = result.provider;

            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{_config["PriceInfoService:BaseUrl"]}?instrumentId={instrumentId}&provider={provider}&interval={_config["PriceInfoService:Interval"]}&periodicity={_config["PriceInfoService:Periodicity"]}&barsCount={_config["PriceInfoService:BarsCount"]}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var jsonData = JsonSerializer.Deserialize<JsonElement>(content);
            var dataArray = jsonData.GetProperty("data").EnumerateArray();

            List<PriceInfo> prices = new List<PriceInfo>();

            foreach(var element in dataArray)
            {
                prices.Add(new PriceInfo
                {
                    symbol = symbol,
                    price = element.GetProperty("c").GetDecimal(),
                    lastupdate = DateTimeOffset.Parse(element.GetProperty("t").GetString()).UtcDateTime
                });
            }
            await _priceInfoRepository.AddRange(prices);

            return  prices;
        }
    }
}
