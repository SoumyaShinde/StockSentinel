using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.Models;
using StockSentinal.Services;

namespace StockSentinal.Jobs
{
    public class StockPricePollingJob
    {
        private readonly IStockService _price;
        private readonly IAlertService _service;

        private readonly AppDbContext _db;
        public StockPricePollingJob(IStockService price, IAlertService service, AppDbContext db)
        {
            _price = price;
            _service = service;
            _db = db;
        }

        public async Task Execute()
        {
            var stocks = await _db.Stocks.ToListAsync();
            foreach(var s in stocks)
            {
                var currentPrice = await _price.GetStockPrices(s.Symbol);
                s.LastPrice = currentPrice;
                s.UpdatedAt = DateTime.Now;
                await Task.Delay(1500); //waits 1.5 seconds between calls
            }
            await _db.SaveChangesAsync();
            await _service.CheckThreshold();
        }
    }
}

