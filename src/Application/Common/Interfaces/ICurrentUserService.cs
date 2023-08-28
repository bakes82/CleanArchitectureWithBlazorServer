namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId     { get; set; }
    string? UserName   { get; set; }
    string? TenantId   { get; set; }
    string? TenantName { get; set; }
}