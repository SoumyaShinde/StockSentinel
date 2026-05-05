using StockSentinal.DTOs;

namespace StockSentinal.Services
{
    public interface IAlertService
    {
        Task<List<AlertResponse>> GetAlert(int userId);
        Task<AlertResponse> CreateAlert(AlertRequest request);
        Task<bool> DeleteAlert(int alertId);

        //System Alert
        Task CheckThreshold();
    }
}