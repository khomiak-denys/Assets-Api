using System.Text.Json;
using System.Net.Http.Headers;
using Assets_Api.Models;
using Assets_Api.Database.Repositiries;

namespace Assets_Api.Services
{
    public class AssetsService
    {
        private readonly HttpClient _httpClient;
        private readonly FintachartsAuthService _authService;
        private readonly AssetsRepository _assetsRepository;

        private readonly IConfiguration _config;
        public AssetsService(HttpClient httpClient, FintachartsAuthService authService, AssetsRepository assetsRepository, IConfiguration config)
        {
            _httpClient = httpClient;
            _authService = authService;
            _assetsRepository = assetsRepository;
            _config = config;
        }
        public async Task<List<Assets>> GetAssetsAsync()
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(_config["AssetsService:BaseUrl"]);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var jsonData = JsonSerializer.Deserialize<JsonElement>(content);
            var dataArray = jsonData.GetProperty("data").EnumerateArray();

            List<Assets> assets = new List<Assets>();

            foreach (var element in dataArray)
            {
                if (element.TryGetProperty("mappings", out var mappings))
                {
                    var mappingKeys = mappings.EnumerateObject().Select(m => m.Name).ToList();
                    var provider = mappingKeys.FirstOrDefault(key => key != "simulation");

                    if (provider != null)
                    {
                        Assets asset = new Assets
                        {
                            id = element.GetProperty("id").GetString(),
                            symbol = element.GetProperty("symbol").GetString(),
                            name = element.GetProperty("description").GetString(),
                            provider = provider
                        };
                        assets.Add(asset);
                        await _assetsRepository.Upsert(asset);
                    }
                }
                
            }
            return assets;
        }
    }
}
