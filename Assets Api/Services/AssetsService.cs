using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using Assets_Api.Models;

namespace Assets_Api.Services
{
    public class AssetsService
    {
        private readonly HttpClient _httpClient;
        private readonly FintachartsAuthService _authService;
        public AssetsService(HttpClient httpClient, FintachartsAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }
        public async Task<List<Assets>> GetAssetsAsync()
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("https://platform.fintacharts.com/api/instruments/v1/instruments?size=1000");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var jsonData = JsonSerializer.Deserialize<JsonElement>(content);
            var dataArray = jsonData.GetProperty("data").EnumerateArray();

            List<Assets> assets = new List<Assets>();

            foreach (var element in dataArray)
            {
                assets.Add(new Assets
                {
                    Id = element.GetProperty("id").GetString(),
                    Symbol = element.GetProperty("symbol").GetString(),
                    Name = element.GetProperty("description").GetString()
                });
            }
            return assets;
        }
    }
}
