using StockSentinal.DTOs;

namespace StockSentinal.Services
{
    public interface IAlertService
    {
        Task<List<AlertResponse>> GetAlert(int userId);
        Task<AlertResponse> CreateAlert(int userId, int stockId, decimal price, string type);
        Task<bool> DeleteAlert(int alertId);

        //System Alert
        Task CheckThreshold();
    }
}