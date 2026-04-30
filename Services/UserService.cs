using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.DTOs;

namespace StockSentinal.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            return await _db.Users.Select(u => new UserResponse(u.Id, u.Email, u.Role, u.CreatedAt)).ToListAsync();
        }
    }
}

