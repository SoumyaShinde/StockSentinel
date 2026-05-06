using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StockSentinal.Data;
using StockSentinal.DTOs;
using System.Text.Json;

namespace StockSentinal.Services
{
    public class StockService : IStockService
    {
        private readonly AppDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public StockService(AppDbContext db, IHttpClientFactory httpClientFactory, IConfiguration config, IMemoryCache cache)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _cache = cache;
        }

        public async Task<List<StockResponse>> GetAllStocks()
        {
            return await _db.Stocks.Select(s => new StockResponse(s.Symbol, s.CompanyName, s.LastPrice)).ToListAsync();
        }

        public async Task<StockResponse?> SearchStockBySymbol(string symbol)
        {
            var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
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
            if (_cache.TryGetValue(symbol, out decimal cachedPrice))
            {
                price = cachedPrice;
            }
            else
            {
                // Call Alpha Vantage
                var client = _httpClientFactory.CreateClient();
                var apiKey = _config["AlphaVantage:ApiKey"];
                var baseUrl = _config["AlphaVantage:BaseUrl"];
                var url = $"{baseUrl}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";

                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Alpha Vantage response: {json}");

                var data = JsonSerializer.Deserialize<AlphaVantageResponse>(json);

                // Handle rate limit!
                if (data?.GlobalQuote == null || string.IsNullOrEmpty(data.GlobalQuote.Price))
                {
                    var dbStock = await _db.Stocks
                        .FirstOrDefaultAsync(s => s.Symbol == symbol);
                    return dbStock?.LastPrice ?? 0m;
                }

                price = decimal.Parse(data.GlobalQuote.Price);
                _cache.Set(symbol, price, TimeSpan.FromSeconds(30));
            }

            // Always update DB with latest price!
            var stock = await _db.Stocks
                .FirstOrDefaultAsync(s => s.Symbol == symbol);
            if (stock != null)
            {
                stock.LastPrice = price;
                stock.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return price;
        }
    }
}