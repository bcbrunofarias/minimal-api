using System.Text.Json.Serialization;

namespace Minimal.API.Responses;

public record RefreshTokenResponse(
    [property: JsonPropertyName("access_token")]
    string AccessToken, 
    
    [property: JsonPropertyName("refresh_token")]
    string RefreshToken);