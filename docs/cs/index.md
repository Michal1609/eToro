# Dokumentace

Česká dokumentace pro eToro .NET client.

- [Začínáme](getting-started.md)
- [Konfigurace](configuration.md)
- [API reference](api-reference.md)
- [Error handling](error-handling.md)
- [Rate limiting a retry](rate-limiting.md)
- [Výpočty účtu](account-calculations.md)
- [Testování a přispívání](testing-and-contributing.md)

Hlavní balíček: `grznarai.etoro`

Hlavní služby:

- `IEToroClient`
- `IEToroCalculationService`

Hlavní registrační metoda:

```csharp
builder.Services.AddEToro(builder.Configuration);
```
