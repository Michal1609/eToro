using EToro.Models.Identity;

namespace EToro.Client;

public sealed partial class EToroClient
{
    public async Task<UserIdentityResponse> GetIdentityAsync(CancellationToken ct = default)
    {
        return await GetFromJsonAsync<UserIdentityResponse>(
            "me",
            "Empty identity response.",
            ct).ConfigureAwait(false);
    }
}
