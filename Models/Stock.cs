namespace StockSentinal.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Stock
    {
        [Key] 
        public int Id { get; set; }

        [Required] public string Symbol { get; set; } = string.Empty;

        [Required] public string CompanyName { get; set; } = string.Empty;

        public decimal LastPrice { get; set; } = 0m;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}

