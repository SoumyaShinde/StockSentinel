using StockSentinal.DTOs;

namespace StockSentinal.Services
{
    public class WatchlistService : IWatchlistService
    {
        public Task<WatchlistResponse> AddWatchList(int userId, int stockid)
        {
            throw new NotImplementedException();
        }
        public Task<List<WatchlistResponse>> GetWatchList(int userId)
        {
            throw new NotImplementedException();
        }
        public Task<bool> RemoveWatchlist(int watchlistId)
        {
            throw new NotImplementedException();
        }
    }
}

