namespace Assets_Api.Models
{
    public class PriceInfo
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdate { get; set; }  
    }
}
