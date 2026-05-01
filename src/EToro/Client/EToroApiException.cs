using System.Net;

namespace EToro.Client;

public sealed class EToroApiException : HttpRequestException
{
    public EToroApiException(
        HttpStatusCode statusCode,
        string endpoint,
        string? responseBody,
        string? requestId,
        TimeSpan? retryAfter)
        : base(CreateMessage(statusCode, endpoint, requestId, retryAfter), null, statusCode)
    {
        StatusCode = statusCode;
        Endpoint = endpoint;
        ResponseBody = responseBody;
        RequestId = requestId;
        RetryAfter = retryAfter;
    }

    public new HttpStatusCode StatusCode { get; }
    public string Endpoint { get; }
    public string? ResponseBody { get; }
    public string? RequestId { get; }
    public TimeSpan? RetryAfter { get; }

    private static string CreateMessage(
        HttpStatusCode statusCode,
        string endpoint,
        string? requestId,
        TimeSpan? retryAfter)
    {
        var message = $"eToro API request failed with status {(int)statusCode} ({statusCode}) for '{endpoint}'.";

        if (!string.IsNullOrWhiteSpace(requestId))
            message += $" RequestId: {requestId}.";

        if (retryAfter.HasValue)
            message += $" RetryAfter: {retryAfter.Value}.";

        return message;
    }
}
