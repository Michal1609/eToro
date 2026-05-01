namespace EToro.Models.Identity;

public sealed record UserIdentityResponse(
    int Gcid,
    int RealCid,
    int DemoCid
);
