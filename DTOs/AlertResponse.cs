namespace StockSentinal.DTOs
{
    public record AlertResponse(int AlertId, string AlertType,string CompanyName, decimal TargetPrice, bool IsTriggered, 
        string Symbol, DateTime? TriggeredAt, DateTime CreatedAt);
}