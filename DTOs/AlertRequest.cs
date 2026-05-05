using System.ComponentModel.DataAnnotations;

namespace StockSentinal.DTOs
{
    public record AlertRequest(
        [Required]
        int UserId, 
        
        [Required]
        int StockId, 
        
        [Required]
        decimal Price, 
        
        [Required]
        string Type);
}

