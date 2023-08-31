using CleanArchitecture.Blazor.Application.Constants.ClaimTypes;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Email);
    }

    public static string? GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);
    }

    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Name);
    }

    public static string? GetProvider(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Provider);
    }

    public static string? GetDisplayName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
    }

    public static string? GetProfilePictureDataUrl(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.ProfilePictureDataUrl);
    }

    public static string? GetSuperiorName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorName);
    }

    public static string? GetSuperiorId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorId);
    }

    public static bool GetStatus(this ClaimsPrincipal claimsPrincipal)
    {
        return Convert.ToBoolean(claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Status));
    }

    public static string? GetAssignRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.AssignedRoles);
    }

    public static string[]? GetRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role)
                              .Select(x => x.Value)
                              .ToArray();
    }
}