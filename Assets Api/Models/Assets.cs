using System.Text.Json.Serialization;

namespace Assets_Api.Models
{
    public class Assets
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        [JsonPropertyName("description")]
        public string Name { get; set; }    
    }
}
