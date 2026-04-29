using Microsoft.EntityFrameworkCore;
using StockSentinal.Data;
using StockSentinal.DTOs;
using StockSentinal.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace StockSentinal.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
                
        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (null == user)
            {
                throw new UnauthorizedAccessException("Invalid Credentials. User not found!");
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid Credentials! Incorrect password");
            }

            var token = GenerateToken(user);
            return new AuthResponse(token, user.Email, user.Role);
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(u=>u.Email == request.Email);
            if (existingUser != null)
            {
                return null!;
            }

            var newUser = new User
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(newUser);

            await _db.SaveChangesAsync();
            var token = GenerateToken(newUser);
            return new AuthResponse(token, request.Email, request.Role);
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["JwtSettings:ExpiryMinutes"]!)
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
    }
    }
}


