namespace Minimal.API.Responses;

public record GetMeResponse(string Name, string Email, List<string> Roles, List<GetMeClaimResponse> Claims);

public record GetMeClaimResponse(string Type, string Value);