namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;

public class RefreshTokenRequest
{
    public RefreshTokenRequest(string token, string refreshToken)
    {
        Token        = token;
        RefreshToken = refreshToken;
    }

    public string Token        { get; set; }
    public string RefreshToken { get; set; }
}