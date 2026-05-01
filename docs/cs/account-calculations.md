# Výpočty účtu

`IEToroCalculationService` poskytuje pomocné výpočty nad PnL, portfoliem a historií obchodů.

## Registrace

Služba se registruje automaticky přes:

```csharp
builder.Services.AddEToro(builder.Configuration);
```

## Použití

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

## Metriky

| Metrika | Význam |
| --- | --- |
| `AvailableCash` | Credit minus manuální pending orders a MIT orders. |
| `InvestedPrincipal` | Částky otevřených pozic, principal copy portfolií, manuální pending orders s náklady a MIT orders. |
| `UnrealizedPnL` | Nerealizované PnL z otevřených manuálních a copy pozic. |
| `RealizedPnL` | Součet `NetProfit` uzavřených obchodů za zadaný rozsah trade history. |
| `Equity` | Available cash plus invested principal plus unrealized PnL. |
| `NetDeposits` | Dopočteno jako `Equity - TotalReturn`. |
| `TotalReturn` | Realized PnL plus unrealized PnL. |
| `TotalReturnPct` | `TotalReturn / NetDeposits * 100`, nebo `0`, když jsou net deposits nulové. |

## Přímé výpočty

Služba nabízí i přímé výpočty, pokud už máte `PnlResponse` nebo closed trades:

```csharp
var availableCash = calculations.CalculateAvailableCash(pnl);
var invested = calculations.CalculateInvestedPrincipal(pnl);
var unrealized = calculations.CalculateUnrealizedPnL(pnl);
var realized = calculations.CalculateRealizedProfit(history.Trades);
```

## Bezpečné stránkování

`GetAccountMetricsAsync` čte historii uzavřených obchodů s omezeným stránkováním. Parametry `pageSize` a `maxPages` určují API zátěž a maximální rozsah skenování.
