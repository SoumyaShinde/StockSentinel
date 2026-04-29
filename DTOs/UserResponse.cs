namespace StockSentinal.DTOs
{
    public record UserResponse
    (
        int Id,
        string Email,
        string Role,
        DateTime CreatedAt
    );
}

