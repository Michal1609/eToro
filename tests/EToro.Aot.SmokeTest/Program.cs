using EToro.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["EToroOptions:ApiKey"] = "aot-smoke-api-key",
        ["EToroOptions:UserKey"] = "aot-smoke-user-key",
        ["EToroOptions:Environment"] = "demo",
        ["EToroOptions:BaseAddress"] = "https://public-api.etoro.com/api/v1/",
        ["EToroOptions:AllowCustomBaseAddress"] = "false",
        ["EToroOptions:Timeout"] = "00:01:40",
        ["EToroOptions:UserAgent"] = "EToro.Aot.SmokeTest",
        ["EToroOptions:RateLimit:Enabled"] = "true",
        ["EToroOptions:RateLimit:PermitLimit"] = "60",
        ["EToroOptions:RateLimit:Window"] = "00:01:00",
        ["EToroOptions:RateLimit:MaxRetries"] = "0",
        ["EToroOptions:RateLimit:DefaultRetryDelay"] = "00:01:00",
        ["EToroOptions:RateLimit:MaxRetryDelay"] = "00:01:00",
        ["EToroOptions:RateLimit:RetryJitterRatio"] = "0.1",
        ["EToroOptions:RateLimit:RetryNonIdempotentRequests"] = "false",
        ["EToroOptions:ErrorHandling:IncludeResponseBody"] = "true",
        ["EToroOptions:ErrorHandling:RedactResponseBody"] = "true",
        ["EToroOptions:ErrorHandling:MaxResponseBodyLength"] = "4096"
    })
    .Build();

var services = new ServiceCollection();
services.AddEToro(configuration);

await using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateOnBuild = true,
    ValidateScopes = true
});

_ = provider.GetRequiredService<IEToroClient>();
Console.WriteLine("EToro AOT smoke test initialized.");
