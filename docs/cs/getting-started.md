# Začínáme

## Požadavky

- .NET 9 SDK nebo novější.
- eToro API key a user key.
- Aplikace používající dependency injection a `IHttpClientFactory`.

## Instalace

```powershell
dotnet add package grznarai.etoro
```

## Konfigurace přístupů

Přihlašovací údaje ukládejte mimo source control:

```powershell
dotnet user-secrets set "EToroOptions:ApiKey" "<api-key>"
dotnet user-secrets set "EToroOptions:UserKey" "<user-key>"
```

Nebo použijte environment proměnné:

```powershell
$env:EToroOptions__ApiKey="<api-key>"
$env:EToroOptions__UserKey="<user-key>"
```

## Registrace služeb

```csharp
using EToro.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEToro(builder.Configuration);
```

Registrace přidá:

- `IEToroClient` jako typed `HttpClient`.
- Auth hlavičky `x-api-key`, `x-user-key` a `x-request-id`.
- Rate-limit a retry handling.
- `IEToroCalculationService`.

## Volání API

```csharp
using EToro.Client;
using EToro.Models.Common;

var portfolio = await client.GetPortfolioAsync(EToroEnvironment.Real, ct);
var pnl = await client.GetPnlAsync(EToroEnvironment.Real, ct);
var rates = await client.GetRatesAsync([100000], ct);
```

Historie obchodů:

```csharp
var history = await client.GetTradeHistoryAsync(
    minDate: new DateOnly(2024, 1, 1),
    page: 0,
    pageSize: 100,
    ct);
```

## Demo vs Real účet

Endpointy, které pracují s účtem, přijímají `EToroEnvironment.Real` nebo `EToroEnvironment.Demo`.

```csharp
var realPnl = await client.GetPnlAsync(EToroEnvironment.Real, ct);
var demoPnl = await client.GetPnlAsync(EToroEnvironment.Demo, ct);
```
