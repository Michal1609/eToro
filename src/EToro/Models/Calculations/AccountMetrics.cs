namespace EToro.Models.Calculations;

public record AccountMetrics(
    decimal AvailableCash,
    decimal InvestedPrincipal,
    decimal UnrealizedPnL,
    decimal RealizedPnL,
    decimal Equity,
    decimal NetDeposits,
    decimal TotalReturn,
    decimal TotalReturnPct
)
{
    public decimal TotalInvested => InvestedPrincipal;
    public decimal RealizedProfit => RealizedPnL;
    public decimal TotalAccountPerformance => TotalReturn;
}
