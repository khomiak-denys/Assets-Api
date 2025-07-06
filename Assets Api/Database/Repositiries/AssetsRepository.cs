using Assets_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Assets_Api.Database.Repositiries
{
    public class AssetsRepository
    {
        private readonly AppDbContext _context;
        public AssetsRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Assets> GetAssetBySymbolAsync(string symbol)
        {
            var asset = await _context.assets_data.FindAsync(symbol);
            if (asset == null)
            {
                
                asset = await _context.assets_data
                    .FirstOrDefaultAsync(a => a.symbol == symbol);
                if (asset == null)
                {
                    Console.WriteLine($"No asset found for symbol: {symbol}");
                }
            }
            return asset;
        }
        public async Task Upsert(Assets asset)
        {
            var existingAsset = await _context.assets_data
                        .FirstOrDefaultAsync(a => a.symbol == asset.symbol);

            if (existingAsset == null)
            {
                _context.assets_data.Add(asset);
            }
            else
            {
                existingAsset.id = asset.id;
                existingAsset.symbol = asset.symbol;
                existingAsset.name = asset.name;
                existingAsset.provider = asset.provider;

                _context.assets_data.Update(existingAsset);
            }
            await _context.SaveChangesAsync();
        }
    }
}
