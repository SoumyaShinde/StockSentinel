namespace StockSentinal.DTOs
{
    public record StockResponse(string Symbol, string CompanyName, decimal LastPrice);
}