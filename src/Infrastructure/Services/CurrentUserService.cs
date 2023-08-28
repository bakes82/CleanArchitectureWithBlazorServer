namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public string? UserId     { get; set; }
    public string? UserName   { get; set; }
    public string? TenantId   { get; set; }
    public string? TenantName { get; set; }
}