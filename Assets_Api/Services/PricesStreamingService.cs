using Assets_Api.Database.Repositiries;
using Assets_Api.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Assets_Api.Services
{
    public class PricesStreamingService
    {
        private readonly FintachartsAuthService _authService;
        private readonly AssetsRepository _assetsRepository;
        private readonly PriceInfoRepository _priceInfoRepository;

        private readonly IConfiguration _config;
        private readonly ILogger<PricesStreamingService> _logger;

        private ClientWebSocket _client;
        private CancellationTokenSource _cts;

        public PricesStreamingService(FintachartsAuthService authService,AssetsRepository assetsRepository, PriceInfoRepository priceInfoRepository, ILogger<PricesStreamingService> logger, IConfiguration config)
        {
            _authService = authService;
            _assetsRepository = assetsRepository;
            _priceInfoRepository = priceInfoRepository;
            _logger = logger;
            _config = config;
        }

        public async Task StartStreamingAsync(List<string> symbols, Action<string, decimal, DateTime> onPriceUpdate)
        {
            _cts = new CancellationTokenSource();
            var accessToken = await _authService.GetAccessTokenAsync();
            var url = _config["PricesStreamingService:BaseUrl"] + accessToken;

            List<Assets> assets = new List<Assets>();
            foreach (var symbol in symbols)
            {
                if (string.IsNullOrEmpty(symbol))
                {
                    _logger.LogWarning("Empty symbol provided, skipping.");
                    continue;
                }
                var asset = await _assetsRepository.GetAssetBySymbolAsync(symbol);
                assets.Add(asset);
            }
            
            if (!assets.Any())
            {
                _logger.LogWarning("No assets found for streaming.");
                return;
            }

            _client = new ClientWebSocket();
            try
            {
                await _client.ConnectAsync(new Uri(url), _cts.Token);
                _logger.LogInformation("WebSocket connected.");

                
                await SubscribeToInstruments(assets);

                
                _ = Task.Run(() => SendPingAsync(_cts.Token));

                
                await ReceiveMessagesAsync(onPriceUpdate, _cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket error.");
            }
        }

        private async Task SubscribeToInstruments(List<Assets> assets)
        {
            foreach (var asset in assets)
            {
                var msg = JsonSerializer.Serialize(new
                {
                    action = "subscribe",
                    instrumentId = asset.id,
                    provider = asset.provider,
                    type = "bars",
                    interval = "1",
                    periodicity = "minute"
                });
                var bytes = Encoding.UTF8.GetBytes(msg);
                await _client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
                _logger.LogInformation($"Subscribed to: {asset.symbol} ({asset.id})");
            }
        }

        private async Task SendPingAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                var pingMsg = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { action = "ping" }));
                await _client.SendAsync(new ArraySegment<byte>(pingMsg), WebSocketMessageType.Text, true, cancellationToken);
                _logger.LogInformation("Ping sent.");
            }
        }

        private async Task ReceiveMessagesAsync(Action<string, decimal, DateTime> onPriceUpdate, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            while (_client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var jsonData = JsonSerializer.Deserialize<JsonElement>(message);

                    if (jsonData.TryGetProperty("type", out var typeElement)) continue; // Пропускаємо "multi-session", "session"

                    if (jsonData.TryGetProperty("symbol", out var symbolElement) &&
                        jsonData.TryGetProperty("close", out var closeElement) &&
                        jsonData.TryGetProperty("timestamp", out var timestampElement))
                    {
                        var symbol = symbolElement.GetString();
                        var price = closeElement.GetDecimal();
                        var timestamp = DateTimeOffset.Parse(timestampElement.GetString()).UtcDateTime;
                        onPriceUpdate?.Invoke(symbol, price, timestamp);

                        await _priceInfoRepository.Add(new PriceInfo
                        {
                            symbol = symbol,
                            price = price,
                            lastupdate = timestamp
                        });
                        _logger.LogInformation($"Streamed price: {symbol} = {price:F2} at {timestamp}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error receiving WebSocket message.");
                }
            }
        }

        public async Task StopStreamingAsync()
        {
            if (_client?.State == WebSocketState.Open)
            {
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                _cts?.Cancel();
                _client?.Dispose();
                _logger.LogInformation("WebSocket stopped.");
            }
        }
    }
}
