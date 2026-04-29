using System.ComponentModel.DataAnnotations;

namespace StockSentinal.DTOs
{
    public record LoginRequest(
        [Required]
        [EmailAddress]
        string Email,
        
        [Required]
        [MinLength(6)]
        [MaxLength(10)]
        string Password);
}