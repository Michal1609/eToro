using System.Text.Json.Serialization;

namespace EToro.Models.Market;

public record ExchangesResponse(
    [property: JsonPropertyName("exchangeInfo")] IReadOnlyList<ExchangeInfo> ExchangeInfo
);

public record ExchangeInfo(
    [property: JsonPropertyName("exchangeID")]          int ExchangeId,
    [property: JsonPropertyName("exchangeDescription")] string? ExchangeDescription
);
