namespace EToro.Configuration;

public sealed class EToroOptions
{
    public const string DefaultBaseAddress = "https://public-api.etoro.com/api/v1/";
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100);

    public string ApiKey { get; set; } = string.Empty;
    public string UserKey { get; set; } = string.Empty;
    public string Environment { get; set; } = "real";
    public Uri BaseAddress { get; set; } = new(DefaultBaseAddress);
    public bool AllowCustomBaseAddress { get; set; }
    public TimeSpan Timeout { get; set; } = DefaultTimeout;
    public string UserAgent { get; set; } = "EToro";
    public RateLimitOptions RateLimit { get; set; } = new();
    public ErrorHandlingOptions ErrorHandling { get; set; } = new();

    public string EnvironmentSegment =>
        Environment.Equals("demo", StringComparison.OrdinalIgnoreCase) ? "demo" : "real";

    public override string ToString() =>
        $"EToroOptions {{ ApiKey=[REDACTED], UserKey=[REDACTED], Environment={Environment}, BaseAddress={BaseAddress}, Timeout={Timeout}, UserAgent={UserAgent} }}";
}
