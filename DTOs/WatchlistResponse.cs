namespace StockSentinal.DTOs
{
    public record WatchlistResponse(int Id, string Symbol, string CompanyName, decimal CurrentPrice, DateTime AddedAt);
}