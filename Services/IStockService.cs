using StockSentinal.DTOs;

namespace StockSentinal.Services
{
    public interface IStockService
    {
        Task<StockResponse?> SearchStockBySymbol(string symbol);
        Task<List<StockResponse>> GetAllStocks();
        Task<decimal> GetStockPrices(string symbol);
    }
}

