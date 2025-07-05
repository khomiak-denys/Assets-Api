using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Assets_Api.Services
{
    public class FintachartsAuthService
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private DateTime _tokenExpiration;
        private string _refreshToken; 
        private DateTime _refreshTokenExpiration;

        private readonly IConfiguration _config;

        public FintachartsAuthService(HttpClient httpClient, IConfiguration config)
        {   
            _httpClient = httpClient;
            _accessToken = string.Empty;
            _tokenExpiration = DateTime.MinValue;
            _refreshToken = string.Empty;
            _config = config;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
            {
                var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, _config["Auth:BaseUrl"]);

                var requestBody = new List<KeyValuePair<string, string>>();
                requestBody.Add(new("grant_type", "password"));
                requestBody.Add(new("client_id", "app-cli"));
                requestBody.Add(new("username", _config["Auth:Username"]));
                requestBody.Add(new("password", _config["Auth:Password"]));

                request.Content = new FormUrlEncodedContent(requestBody);
                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(await response.Content.ReadAsStringAsync());

                if(tokenResponse != null)
                {
                    _accessToken = tokenResponse.AccessToken;
                    _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                    _refreshTokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.RefreshExpiresIn);
                    _refreshToken = tokenResponse.RefreshToken;
                }     
            }
            return _accessToken;
        }
        public class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            [JsonPropertyName("expites_in")]
            public int ExpiresIn { get; set; }
            [JsonPropertyName("refresh_expires_in")]
            public int RefreshExpiresIn { get; set; }
            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }
        }
    }
}
