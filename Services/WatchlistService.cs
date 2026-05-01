using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.DTOs;
using StockSentinal.Models;

namespace StockSentinal.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly AppDbContext _db;

        public WatchlistService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<WatchlistResponse>> GetWatchList(int userId)
        {
            return await _db.WatchlistItems
                .Include(w => w.Stock)
                .Where(w => w.UserId == userId)
                .Select(s => new WatchlistResponse(s.Id, s.Stock.Symbol, s.Stock.CompanyName, s.Stock.LastPrice, s.Stock.UpdatedAt))
                .ToListAsync();            
        }

        public async Task<WatchlistResponse> AddWatchList(int userId, int stockId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.Id == stockId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User Not found");
            }

            if (stock == null)
            {
                throw new UnauthorizedAccessException("no stock detail found for the id:" + stockId);
            }

            var watchlistCreate = new WatchlistItem
            {
                UserId = userId,
                User = user,
                Stock = stock,
                StockId = stockId,
                AddedAt = DateTime.Now
            };
            _db.WatchlistItems.Add(watchlistCreate);
            await _db.SaveChangesAsync();

            return new WatchlistResponse(
                watchlistCreate.Id,
               stock.Symbol,
               stock.CompanyName,
               stock.LastPrice,
               stock.UpdatedAt
            );
        }
        public async Task<bool> RemoveWatchlist(int watchlistId)
        {
            var found = await _db.WatchlistItems.FirstOrDefaultAsync(w => w.Id == watchlistId);
            if(found == null)
            {
                return false;
            }
            _db.WatchlistItems.Remove(found);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

