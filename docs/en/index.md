# Documentation

English documentation for the eToro .NET client.

- [Getting started](getting-started.md)
- [Configuration](configuration.md)
- [API reference](api-reference.md)
- [Error handling](error-handling.md)
- [Rate limiting and retries](rate-limiting.md)
- [Account calculations](account-calculations.md)
- [Testing and contributing](testing-and-contributing.md)

Main package: `grznarai.etoro`

Main services:

- `IEToroClient`
- `IEToroCalculationService`

Main registration method:

```csharp
builder.Services.AddEToro(builder.Configuration);
```
