namespace EToro.Configuration;

public sealed class ErrorHandlingOptions
{
    public const int DefaultMaxResponseBodyLength = 4096;

    public bool IncludeResponseBody { get; set; } = true;
    public bool RedactResponseBody { get; set; } = true;
    public int? MaxResponseBodyLength { get; set; } = DefaultMaxResponseBodyLength;
}
