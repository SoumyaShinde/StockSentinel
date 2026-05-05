using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.DTOs;
using StockSentinal.Models;

namespace StockSentinal.Services
{
    public class AlertService : IAlertService
    {
        private readonly AppDbContext _db;
        public AlertService(AppDbContext db)
        {
            _db = db;
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
        public Task CheckThreshold()
        {
            throw new NotImplementedException();
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
                TriggeredAt = DateTime.UtcNow,
                IsTriggered = false,
            };
            _db.PriceAlerts.Add(pa);
            await _db.SaveChangesAsync();

            return (new AlertResponse(pa.Id, pa.AlertType,pa.Stock.CompanyName,pa.TargetPrice,true,pa.Stock.Symbol,pa.TriggeredAt,pa.CreatedAt));
        }

        public Task<bool> DeleteAlert(int alertId)
        {
            throw new NotImplementedException();
        }
    }
}