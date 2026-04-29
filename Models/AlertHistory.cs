using System.ComponentModel.DataAnnotations;

namespace StockSentinal.Models
{
    public class AlertHistory
    {
        [Key] 
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int StockId { get; set;}

        public Stock Stock { get; set; } = null!;

        public string Message { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    }
}

