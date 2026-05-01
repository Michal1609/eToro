using System.Text.Json;
using System.Text.Json.Serialization;

namespace EToro.Models.UserInfo;

public record UserGainResponse(
    [property: JsonPropertyName("monthly")] IReadOnlyList<UserGainEntry>? Monthly,
    [property: JsonPropertyName("yearly")] IReadOnlyList<UserGainEntry>? Yearly
);

public record UserGainEntry(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("gain")] decimal Gain
);

[JsonConverter(typeof(UserDailyGainResponseJsonConverter))]
public record UserDailyGainResponse(
    IReadOnlyList<UserGainEntry>? Daily,
    decimal? Gain
)
{
    public bool IsDaily => Daily is not null;
    public bool IsPeriod => Gain.HasValue;
}

internal sealed class UserDailyGainResponseJsonConverter : JsonConverter<UserDailyGainResponse>
{
    public override UserDailyGainResponse Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var entries = JsonSerializer.Deserialize<IReadOnlyList<UserGainEntry>>(ref reader, options);
            return new UserDailyGainResponse(entries, null);
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var gain = document.RootElement.TryGetProperty("gain", out var gainElement)
                ? gainElement.GetDecimal()
                : (decimal?)null;

            return new UserDailyGainResponse(null, gain);
        }

        throw new JsonException("Unexpected daily gain response shape.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        UserDailyGainResponse value,
        JsonSerializerOptions options)
    {
        if (value.Daily is not null)
        {
            JsonSerializer.Serialize(writer, value.Daily, options);
            return;
        }

        writer.WriteStartObject();
        if (value.Gain.HasValue)
            writer.WriteNumber("gain", value.Gain.Value);
        writer.WriteEndObject();
    }
}
