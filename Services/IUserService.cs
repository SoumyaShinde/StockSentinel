using StockSentinal.DTOs;

namespace StockSentinal.Services
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetAllUsersAsync();
    }
}

