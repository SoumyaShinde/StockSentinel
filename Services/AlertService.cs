using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.DTOs;
using StockSentinal.Models;

namespace StockSentinal.Services
{
    public class AlertService(AppDbContext db, IStockService price) : IAlertService
    {
        public async Task<List<AlertResponse>> GetAlert(int userId)
        {
            return await db.PriceAlerts
                .Include(a => a.Stock)
                .Where(a => a.UserId == userId)
                .Select(s => new AlertResponse(s.Id, s.AlertType, s.Stock.CompanyName,
                    s.TargetPrice, s.IsTriggered, s.Stock.Symbol, s.TriggeredAt, s.CreatedAt))
                .ToListAsync();
        }

        public async Task<AlertResponse> CreateAlert(AlertRequest request)
        {
            var user = await db.Users.FirstOrDefaultAsync(a => a.Id == request.UserId);
            var stock = await db.Stocks.FirstOrDefaultAsync(s => s.Id == request.StockId);
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
            db.PriceAlerts.Add(pa);
            await db.SaveChangesAsync();

            return (new AlertResponse(pa.Id, pa.AlertType,pa.Stock.CompanyName,pa.TargetPrice,pa.IsTriggered,pa.Stock.Symbol,pa.TriggeredAt,pa.CreatedAt));
        }

        public async Task<bool> DeleteAlert(int alertId)
        {
            var alert = await db.PriceAlerts.FirstOrDefaultAsync(a => a.Id == alertId);
            if (alert != null)
            {
                db.PriceAlerts.Remove(alert);
                await db.SaveChangesAsync();
                return true;
            }

            return false;   
        }

        public async Task CheckThreshold()
        {
            var untriggeredAlerts = await db.PriceAlerts
                .Include(s=>s.Stock)
                .Where(a => !a.IsTriggered).ToListAsync();

            foreach (var alert in untriggeredAlerts)
            {
                var currentPrice = await price.GetStockPrices(alert.Stock.Symbol);
                bool triggered = ((alert.AlertType == "Above" && currentPrice > alert.TargetPrice) ||
                    (alert.AlertType == "Below" && currentPrice < alert.TargetPrice));

                if (!triggered) continue;
                
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

                db.Add(ah);
                await db.SaveChangesAsync();
            }
        }
    }
}