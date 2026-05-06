using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StockSentinal.Data;
using StockSentinal.DTOs;
using System.Text.Json;

namespace StockSentinal.Services
{
    public class StockService(AppDbContext db, IHttpClientFactory httpClientFactory, IConfiguration config, IMemoryCache cache) : IStockService
    {
        public async Task<List<StockResponse>> GetAllStocks()
        {
            return await db.Stocks.Select(s => new StockResponse(s.Symbol, s.CompanyName, s.LastPrice)).ToListAsync();
        }

        public async Task<StockResponse?> SearchStockBySymbol(string symbol)
        {
            var stock = await db.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
            if (stock == null)
            {
                return null;
            }
            return new StockResponse(stock.Symbol, stock.CompanyName, stock.LastPrice);
        }

        public async Task<decimal> GetStockPrices(string symbol)
        {
            decimal price;

            // Check cache first
            if (cache.TryGetValue(symbol, out decimal cachedPrice))
            {
                price = cachedPrice;
            }
            else
            {
                // Call Alpha Vantage
                var client = httpClientFactory.CreateClient();
                var apiKey = config["AlphaVantage:ApiKey"];
                var baseUrl = config["AlphaVantage:BaseUrl"];
                var url = $"{baseUrl}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";

                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Alpha Vantage response: {json}");

                var data = JsonSerializer.Deserialize<AlphaVantageResponse>(json);

                // Handle rate limit!
                if (data?.GlobalQuote == null || string.IsNullOrEmpty(data.GlobalQuote.Price))
                {
                    var dbStock = await db.Stocks
                        .FirstOrDefaultAsync(s => s.Symbol == symbol);
                    return dbStock?.LastPrice ?? 0m;
                }

                price = decimal.Parse(data.GlobalQuote.Price);
                cache.Set(symbol, price, TimeSpan.FromSeconds(30));
            }

            // Always update DB with latest price!
            var stock = await db.Stocks
                .FirstOrDefaultAsync(s => s.Symbol == symbol);
            if (stock != null)
            {
                stock.LastPrice = price;
                stock.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            return price;
        }
    }
}