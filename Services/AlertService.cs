using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.DTOs;

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
                .Select(s => new AlertResponse(s.Id, s.AlertType, s.Stock.CompanyName, s.TargetPrice, s.IsTriggered, s.Stock.Symbol, s.TriggeredAt, s.CreatedAt))
                .ToListAsync();
        }
        public Task CheckThreshold()
        {
            throw new NotImplementedException();
        }

        public Task<AlertResponse> CreateAlert(int userId, int stockId, decimal price, string type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAlert(int alertId)
        {
            throw new NotImplementedException();
        }
    }
}