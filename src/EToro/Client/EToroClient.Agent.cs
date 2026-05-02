using EToro.Models.Agent;

namespace EToro.Client;

public sealed partial class EToroClient
{
    public async Task<AgentPortfolioResponse> GetAgentPortfoliosAsync(CancellationToken ct = default)
    {
        return await GetFromJsonAsync<AgentPortfolioResponse>(
            "agent-portfolios",
            "Empty agent portfolios response.",
            ct).ConfigureAwait(false);
    }
}
