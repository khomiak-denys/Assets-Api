using Assets_Api.Models;
using System.Diagnostics;

namespace Assets_Api.Database.Repositiries
{
    public class PriceInfoRepository
    {
        private readonly AppDbContext _context;

        public PriceInfoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddRange(List<PriceInfo> prices)
        {
            _context.priceInfo_data.AddRange(prices);
            await _context.SaveChangesAsync();
        }
        public async Task Add(PriceInfo priceInfo)
        {
            _context.priceInfo_data.Add(priceInfo);
            await _context.SaveChangesAsync();
        }
    }
}
