namespace Minimal.API.Auth.Token;

public record TokenValidationResult(bool IsValid, string? Username = null);