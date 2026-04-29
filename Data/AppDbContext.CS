using StockSentinal.Models;
using Microsoft.EntityFrameworkCore;

namespace StockSentinal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users{ get; set; }
        public DbSet<Stock> Stocks{ get; set; }
        public DbSet<WatchlistItem> WatchlistItems { get; set; }
        public DbSet<PriceAlert> PriceAlerts{ get; set; }
        public DbSet<AlertHistory> AlertHistory{ get; set; }
    }
}