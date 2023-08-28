namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;

public class TokenResponse
{
    public string?   Token                  { get; set; }
    public string?   RefreshToken           { get; set; }
    public string?   ProfilePictureDataUrl  { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}