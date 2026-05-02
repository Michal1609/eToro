using System.Text.Json.Serialization;
using EToro.Models.Agent;
using EToro.Models.Feed;
using EToro.Models.Identity;
using EToro.Models.Market;
using EToro.Models.PiData;
using EToro.Models.Pnl;
using EToro.Models.Portfolio;
using EToro.Models.Social;
using EToro.Models.Trades;
using EToro.Models.UserInfo;
using EToro.Models.Watchlist;

namespace EToro.Client;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, MaxDepth = 32)]
[JsonSerializable(typeof(AgentPortfolioResponse))]
[JsonSerializable(typeof(DiscussionsResponse))]
[JsonSerializable(typeof(UserIdentityResponse))]
[JsonSerializable(typeof(ExchangesResponse))]
[JsonSerializable(typeof(InstrumentTypesResponse))]
[JsonSerializable(typeof(InstrumentMetadataResponse))]
[JsonSerializable(typeof(IReadOnlyList<InstrumentClosingPrice>))]
[JsonSerializable(typeof(StocksIndustriesResponse))]
[JsonSerializable(typeof(InstrumentSearchResponse))]
[JsonSerializable(typeof(LiveRatesResponse))]
[JsonSerializable(typeof(CandleResponse))]
[JsonSerializable(typeof(CopiersResponse))]
[JsonSerializable(typeof(PopularInvestorsResponse))]
[JsonSerializable(typeof(PnlApiResponse))]
[JsonSerializable(typeof(PortfolioApiResponse))]
[JsonSerializable(typeof(List<ClosedTrade>))]
[JsonSerializable(typeof(OrderForOpenInfoResponse))]
[JsonSerializable(typeof(UserProfileResponse))]
[JsonSerializable(typeof(UserSearchResponse))]
[JsonSerializable(typeof(UserGainResponse))]
[JsonSerializable(typeof(UserDailyGainResponse))]
[JsonSerializable(typeof(UserLivePortfolioResponse))]
[JsonSerializable(typeof(UserTradeInfoResponse))]
[JsonSerializable(typeof(CuratedListsResponse))]
[JsonSerializable(typeof(MarketRecommendationsResponse))]
[JsonSerializable(typeof(WatchlistsResponse))]
[JsonSerializable(typeof(IReadOnlyList<WatchlistItemDto>))]
[JsonSerializable(typeof(WatchlistDto))]
internal sealed partial class EToroJsonContext : JsonSerializerContext { }
