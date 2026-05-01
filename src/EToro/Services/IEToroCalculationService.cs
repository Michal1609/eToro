using EToro.Models.Calculations;
using EToro.Models.Common;
using EToro.Models.Pnl;
using EToro.Models.Trades;

namespace EToro.Services;

public interface IEToroCalculationService
{
    decimal CalculateAvailableCash(PnlResponse pnl);
    decimal CalculateTotalInvested(PnlResponse pnl);
    decimal CalculateProfitLoss(PnlResponse pnl);
    decimal CalculateInvestedPrincipal(PnlResponse pnl);
    decimal CalculateUnrealizedPnL(PnlResponse pnl);
    decimal CalculateEquity(PnlResponse pnl);
    decimal CalculateRealizedProfit(IEnumerable<ClosedTrade> trades);
    Task<AccountMetrics> GetAccountMetricsAsync(EToroEnvironment environment, CancellationToken ct);
    Task<AccountMetrics> GetAccountMetricsAsync(
        EToroEnvironment environment,
        DateOnly? fromDate = null,
        int pageSize = EToroCalculationService.DefaultTradeHistoryPageSize,
        int maxPages = EToroCalculationService.DefaultTradeHistoryMaxPages,
        CancellationToken ct = default);
}
