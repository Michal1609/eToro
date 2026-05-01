using System.Text.Json.Serialization;

namespace EToro.Models.Watchlist;

[JsonConverter(typeof(JsonStringEnumConverter<WatchlistType>))]
public enum WatchlistType { Static, Dynamic, RecentlyInvested, Default }
