using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.Services;

namespace StockSentinal.Jobs
{
    public class StockPricePollingJob(IStockService price, IAlertService service, AppDbContext db)
    {
        public async Task Execute()
        {
            var stocks = await db.Stocks.ToListAsync();
            foreach(var s in stocks)
            {
                var currentPrice = await price.GetStockPrices(s.Symbol);
                s.LastPrice = currentPrice;
                s.UpdatedAt = DateTime.Now;
                await Task.Delay(1500); //waits 1.5 seconds between calls
            }
            await db.SaveChangesAsync();
            await service.CheckThreshold();
        }
    }
}

