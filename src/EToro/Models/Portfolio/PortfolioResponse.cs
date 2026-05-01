using System.Text.Json.Serialization;

namespace EToro.Models.Portfolio;

public record PortfolioApiResponse(
    [property: JsonPropertyName("clientPortfolio")] PortfolioResponse ClientPortfolio
);

public record PortfolioResponse(
    [property: JsonPropertyName("positions")] IReadOnlyList<PortfolioPosition> Positions,
    [property: JsonPropertyName("credit")] decimal Credit,
    [property: JsonPropertyName("bonusCredit")] decimal BonusCredit
);
