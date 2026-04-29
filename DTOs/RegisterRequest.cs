using System.ComponentModel.DataAnnotations;

namespace StockSentinal.DTOs
{
        public record RegisterRequest(
            [Required]
            [EmailAddress]
            string Email, 
            
            [Required]
            [MinLength(6)]
            string Password,
            
            string Role="User");
}