namespace EToro.Models.PiData;

public sealed record CopiersResponse(
    IReadOnlyList<CopierInfo> Copiers
);
