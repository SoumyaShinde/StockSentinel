using StockSentinal.DTOs;

namespace StockSentinal.Services
{
    public class StockService : IStockService
    {
        public Task<List<StockResponse>> GetAllStocks()
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetStockPrices(string symbol)
        {
            throw new NotImplementedException();
        }

        public Task<StockResponse?> SearchStockBySymbol(string symbol)
        {
            throw new NotImplementedException();
        }
    }
}