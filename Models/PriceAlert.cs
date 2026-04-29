using System.ComponentModel.DataAnnotations;

namespace StockSentinal.Models
{
    public class PriceAlert
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int StockId { get; set; }

        public Stock Stock { get; set; } = null!;

        public decimal TargetPrice { get; set; } = 0m;

        public string AlertType { get; set; } = string.Empty;

        public bool IsTriggered { get; set; } = false;

        public DateTime? TriggeredAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}