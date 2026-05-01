using EToro.Client;
using EToro.Models.Calculations;
using EToro.Models.Common;
using EToro.Models.Pnl;
using EToro.Models.Trades;

namespace EToro.Services;

public sealed class EToroCalculationService(IEToroClient client) : IEToroCalculationService
{
    public static readonly DateOnly DefaultTradeHistoryFromDate = new(2007, 1, 1);
    public const int DefaultTradeHistoryPageSize = 100;
    public const int DefaultTradeHistoryMaxPages = 1_000;

    public decimal CalculateAvailableCash(PnlResponse pnl)
    {
        ArgumentNullException.ThrowIfNull(pnl);
        var manualPending = pnl.OrdersForOpen
            .Where(o => o.MirrorId == 0)
            .Sum(o => o.Amount);

        var mitOrders = pnl.Orders.Sum(o => o.Amount);

        return pnl.Credit - manualPending - mitOrders;
    }

    public decimal CalculateTotalInvested(PnlResponse pnl)
    {
        return CalculateInvestedPrincipal(pnl);
    }

    public decimal CalculateInvestedPrincipal(PnlResponse pnl)
    {
        ArgumentNullException.ThrowIfNull(pnl);
        var openPositions = pnl.Positions.Sum(p => p.Amount);

        var mirrorPositions = pnl.MirrorPortfolios
            .SelectMany(m => m.Positions)
            .Sum(p => p.Amount);

        // adjustedAvailability = availableAmount - closedPositionsNetProfit per mirror
        var mirrorAdjusted = pnl.MirrorPortfolios
            .Sum(m => m.AvailableAmount - m.ClosedPositionsNetProfit);

        var manualPendingWithCosts = pnl.OrdersForOpen
            .Where(o => o.MirrorId == 0)
            .Sum(o => o.Amount + o.TotalExternalCosts);

        var mitOrders = pnl.Orders.Sum(o => o.Amount);

        return openPositions + mirrorPositions + mirrorAdjusted + manualPendingWithCosts + mitOrders;
    }

    public decimal CalculateProfitLoss(PnlResponse pnl)
    {
        return CalculateUnrealizedPnL(pnl);
    }

    public decimal CalculateUnrealizedPnL(PnlResponse pnl)
    {
        ArgumentNullException.ThrowIfNull(pnl);
        var positionsPnl = pnl.Positions.Sum(p => p.PnL);

        var mirrorPnl = pnl.MirrorPortfolios
            .SelectMany(m => m.Positions)
            .Sum(p => p.PnL);

        return positionsPnl + mirrorPnl;
    }

    public decimal CalculateEquity(PnlResponse pnl)
    {
        return CalculateAvailableCash(pnl) + CalculateInvestedPrincipal(pnl) + CalculateUnrealizedPnL(pnl);
    }

    public decimal CalculateRealizedProfit(IEnumerable<ClosedTrade> trades)
    {
        ArgumentNullException.ThrowIfNull(trades);
        return trades.Sum(t => t.NetProfit);
    }

    public Task<AccountMetrics> GetAccountMetricsAsync(EToroEnvironment environment, CancellationToken ct)
    {
        return GetAccountMetricsAsync(environment, fromDate: null, DefaultTradeHistoryPageSize, DefaultTradeHistoryMaxPages, ct);
    }

    public async Task<AccountMetrics> GetAccountMetricsAsync(
        EToroEnvironment environment,
        DateOnly? fromDate = null,
        int pageSize = DefaultTradeHistoryPageSize,
        int maxPages = DefaultTradeHistoryMaxPages,
        CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxPages);

        var pnl = await client.GetPnlAsync(environment, ct);

        var availableCash = CalculateAvailableCash(pnl);
        var investedPrincipal = CalculateInvestedPrincipal(pnl);
        var unrealizedPnl = CalculateUnrealizedPnL(pnl);
        var realizedPnl = await CalculateRealizedPnLAsync(fromDate ?? DefaultTradeHistoryFromDate, pageSize, maxPages, ct);
        var equity = CalculateEquity(pnl);
        var totalReturn = realizedPnl + unrealizedPnl;
        var netDeposits = equity - totalReturn;
        var totalReturnPct = netDeposits == 0m ? 0m : totalReturn / netDeposits * 100m;

        return new AccountMetrics(
            availableCash,
            investedPrincipal,
            unrealizedPnl,
            realizedPnl,
            equity,
            netDeposits,
            totalReturn,
            totalReturnPct);
    }

    private async Task<decimal> CalculateRealizedPnLAsync(
        DateOnly fromDate,
        int pageSize,
        int maxPages,
        CancellationToken ct)
    {
        var realizedPnl = 0m;
        var seenPageSignatures = new HashSet<string>(StringComparer.Ordinal);
        long? previousLastTradeId = null;

        for (var page = 0; page < maxPages; page++)
        {
            var response = await client.GetTradeHistoryAsync(fromDate, page, pageSize, ct);
            if (response.Trades.Count == 0)
                break;

            var pageSignature = string.Join(",", response.Trades.Select(t => t.PositionId));
            var lastTradeId = response.Trades[^1].PositionId;
            if (!seenPageSignatures.Add(pageSignature) || previousLastTradeId == lastTradeId)
                break;

            realizedPnl += response.Trades.Sum(t => t.NetProfit);
            previousLastTradeId = lastTradeId;

            if (response.Trades.Count < pageSize)
                break;
        }

        return realizedPnl;
    }
}
