using StockSentinal.DTOs;

namespace StockSentinal.Services
{
    public class AlertService : IAlertService
    {
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

        public Task<List<AlertResponse>> GetAlert(int userId)
        {
            throw new NotImplementedException();
        }
    }
}