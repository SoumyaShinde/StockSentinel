using System.Text.Json.Serialization;

namespace StockSentinal.DTOs
{
    public class AlphaVantageResponse
    {
        [JsonPropertyName("Global Quote")] 
        public GlobalQuote? GlobalQuote { get; set; }
    }

    public class GlobalQuote
    {
        [JsonPropertyName("01. symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("02. open")]
        public string Open { get; set; } = string.Empty;

        [JsonPropertyName("03. high")]
        public string High { get; set; } = string.Empty;

        [JsonPropertyName("04. low")]
        public string Low { get; set; } = string.Empty;

        [JsonPropertyName("05. price")]
        public string Price { get; set; } = String.Empty;

        [JsonPropertyName("06. volume")]
        public string Volumn { get; set; } = string.Empty;

        [JsonPropertyName("07. latest trading day")]
        public string LastTradeDay { get; set; } = string.Empty;
    }
}

