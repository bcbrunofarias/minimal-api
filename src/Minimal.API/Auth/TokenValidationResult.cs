namespace Minimal.API.Auth;

public record TokenValidationResult(bool IsValid, string? Username = null);