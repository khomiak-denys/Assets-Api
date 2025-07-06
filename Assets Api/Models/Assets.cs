using System.Text.Json.Serialization;

namespace Assets_Api.Models
{
    public class Assets
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string provider { get; set; }
    }
}
