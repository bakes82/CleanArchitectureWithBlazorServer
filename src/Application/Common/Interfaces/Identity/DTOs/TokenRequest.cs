namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;

public class TokenRequest
{
    public string? UserName   { get; set; }
    public string? Password   { get; set; }
    public bool    RememberMe { get; set; }
}