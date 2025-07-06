namespace Assets_Api.Models
{
    public class PriceInfo
    {
        public long id { get; set; }
        public string symbol { get; set; }
        public decimal price { get; set; }
        public DateTime lastupdate { get; set; }  
    }
}
