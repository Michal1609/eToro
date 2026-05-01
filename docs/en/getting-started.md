# Getting Started

## Requirements

- .NET 9 SDK or newer.
- eToro API key and user key.
- An application using dependency injection and `IHttpClientFactory`.

## Install

```powershell
dotnet add package grznarai.etoro
```

## Add Configuration

Store credentials outside source control:

```powershell
dotnet user-secrets set "EToroOptions:ApiKey" "<api-key>"
dotnet user-secrets set "EToroOptions:UserKey" "<user-key>"
```

Or use environment variables:

```powershell
$env:EToroOptions__ApiKey="<api-key>"
$env:EToroOptions__UserKey="<user-key>"
```

## Register Services

```csharp
using EToro.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEToro(builder.Configuration);
```

The registration adds:

- `IEToroClient` as a typed `HttpClient`.
- Authentication headers for `x-api-key`, `x-user-key`, and `x-request-id`.
- Rate-limit and retry handling.
- `IEToroCalculationService`.

## Call the API

```csharp
using EToro.Client;
using EToro.Models.Common;
using EToro.Models.Portfolio;

public sealed class PortfolioService(IEToroClient client)
{
    public Task<PortfolioResponse> GetRealPortfolioAsync(CancellationToken ct)
    {
        return client.GetPortfolioAsync(EToroEnvironment.Real, ct);
    }
}
```

Prefer direct async calls in your own services:

```csharp
var pnl = await client.GetPnlAsync(EToroEnvironment.Real, ct);
var rates = await client.GetRatesAsync([100000], ct);
var history = await client.GetTradeHistoryAsync(new DateOnly(2024, 1, 1), page: 0, pageSize: 100, ct);
```

## Demo vs Real Account

Endpoints that need an account environment accept `EToroEnvironment.Real` or `EToroEnvironment.Demo`.

```csharp
var realPnl = await client.GetPnlAsync(EToroEnvironment.Real, ct);
var demoPnl = await client.GetPnlAsync(EToroEnvironment.Demo, ct);
```
