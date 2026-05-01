# Account Calculations

`IEToroCalculationService` provides helper calculations on top of PnL, portfolio, and trade-history data.

## Register

The service is registered automatically by:

```csharp
builder.Services.AddEToro(builder.Configuration);
```

## Use

```csharp
using EToro.Models.Common;
using EToro.Services;

var metrics = await calculations.GetAccountMetricsAsync(
    EToroEnvironment.Real,
    fromDate: new DateOnly(2024, 1, 1),
    pageSize: 100,
    maxPages: 50,
    ct);
```

## Metrics

| Metric | Meaning |
| --- | --- |
| `AvailableCash` | Credit minus manual pending orders and MIT orders. |
| `InvestedPrincipal` | Open position amounts, copy portfolio principal, manual pending orders with costs, and MIT orders. |
| `UnrealizedPnL` | Unrealized PnL from open manual and copy positions. |
| `RealizedPnL` | Sum of closed trade `NetProfit` over the requested trade-history range. |
| `Equity` | Available cash plus invested principal plus unrealized PnL. |
| `NetDeposits` | Inferred as `Equity - TotalReturn`. |
| `TotalReturn` | Realized PnL plus unrealized PnL. |
| `TotalReturnPct` | `TotalReturn / NetDeposits * 100`, or `0` when net deposits are zero. |

## Direct Calculation Methods

The service also exposes direct calculation methods when you already have a `PnlResponse` or closed trades:

```csharp
var availableCash = calculations.CalculateAvailableCash(pnl);
var invested = calculations.CalculateInvestedPrincipal(pnl);
var unrealized = calculations.CalculateUnrealizedPnL(pnl);
var realized = calculations.CalculateRealizedProfit(history.Trades);
```

## Paging Safety

`GetAccountMetricsAsync` reads closed trade history with bounded paging. Use `pageSize` and `maxPages` to control API load and how far the service can scan.
