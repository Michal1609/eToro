using System.Text.Json.Serialization;

namespace EToro.Models.Market;

public record InstrumentTypesResponse(
    [property: JsonPropertyName("instrumentTypes")] IReadOnlyList<InstrumentTypeInfo> InstrumentTypes
);

public record InstrumentTypeInfo(
    [property: JsonPropertyName("instrumentTypeID")]          int InstrumentTypeId,
    [property: JsonPropertyName("instrumentTypeDescription")] string? InstrumentTypeDescription
);
