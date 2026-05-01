using StockSentinal.DTOs;
namespace StockSentinal.Services
{
    public interface IWatchlistService
    {
        Task<List<WatchlistResponse>> GetWatchList(int userId);
        Task<WatchlistResponse> AddWatchList(int userId, int stockId);
        Task<bool> RemoveWatchlist(int watchlistId);
    }
}
