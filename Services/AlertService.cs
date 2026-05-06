using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.DTOs;
using StockSentinal.Models;

namespace StockSentinal.Services
{
    public class AlertService : IAlertService
    {
        private readonly AppDbContext _db;
        private readonly IStockService _price;
        public AlertService(AppDbContext db, IStockService price)
        {
            _db = db;
            _price = price;
        }

        public async Task<List<AlertResponse>> GetAlert(int userId)
        {
            return await _db.PriceAlerts
                .Include(a => a.Stock)
                .Where(a => a.UserId == userId)
                .Select(s => new AlertResponse(s.Id, s.AlertType, s.Stock.CompanyName,
                    s.TargetPrice, s.IsTriggered, s.Stock.Symbol, s.TriggeredAt, s.CreatedAt))
                .ToListAsync();
        }

        public async Task<AlertResponse> CreateAlert(AlertRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Id == request.UserId);
            var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.Id == request.StockId);
            if (user == null || stock == null)
            {
                throw new UnauthorizedAccessException("User Not Found");
            }

            var pa = new PriceAlert 
            {
                UserId = user.Id,
                User = user,
                StockId = stock.Id,
                Stock = stock,
                TargetPrice = request.Price,
                AlertType = request.Type,
                CreatedAt = DateTime.UtcNow,
                TriggeredAt = null,
                IsTriggered = false,
            };
            _db.PriceAlerts.Add(pa);
            await _db.SaveChangesAsync();

            return (new AlertResponse(pa.Id, pa.AlertType,pa.Stock.CompanyName,pa.TargetPrice,pa.IsTriggered,pa.Stock.Symbol,pa.TriggeredAt,pa.CreatedAt));
        }

        public async Task<bool> DeleteAlert(int alertId)
        {
            var alert = await _db.PriceAlerts.FirstOrDefaultAsync(a => a.Id == alertId);
            if (alert != null)
            {
                _db.PriceAlerts.Remove(alert);
                await _db.SaveChangesAsync();
                return true;
            }

            return false;   
        }

        public async Task CheckThreshold()
        {
            var untriggeredAlerts = await _db.PriceAlerts
                .Include(s=>s.Stock)
                .Where(a => !a.IsTriggered).ToListAsync();

            foreach (var alert in untriggeredAlerts)
            {
                var currentPrice = await _price.GetStockPrices(alert.Stock.Symbol);
                bool triggered = false;
                if (alert.AlertType == "Above" && currentPrice > alert.TargetPrice)
                {
                    triggered = true;
                }

                if (alert.AlertType == "Below" && currentPrice < alert.TargetPrice)
                {
                    triggered = true;
                }

                if (triggered)
                {
                    alert.TriggeredAt = DateTime.Now;
                    alert.IsTriggered = triggered;
                    alert.Stock.LastPrice = currentPrice;

                    var ah = new AlertHistory
                    {
                        UserId = alert.UserId,
                        User = alert.User,
                        Stock = alert.Stock,
                        TriggeredAt = DateTime.Now,
                        Message = $"{alert.Stock.Symbol} hit your {alert.AlertType} target of {alert.TargetPrice}!",

                    };

                    _db.Add(ah);
                    await _db.SaveChangesAsync();
                }
            }
        }
    }
}