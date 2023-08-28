using System.Security.Cryptography;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public class AccessTokenProvider
{
    private readonly ICurrentUserService   _currentUser;
    private readonly IIdentityService      _identityService;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager     _navigation;
    private readonly ITenantProvider       _tenantProvider;
    private readonly string                _tokenKey = nameof(_tokenKey);

    public AccessTokenProvider(ProtectedLocalStorage localStorage, NavigationManager navigation, IIdentityService identityService, ITenantProvider tenantProvider, ICurrentUserService currentUser)
    {
        _localStorage    = localStorage;
        _navigation      = navigation;
        _identityService = identityService;
        _tenantProvider  = tenantProvider;
        _currentUser     = currentUser;
    }

    public string? AccessToken { get; private set; }

    public async Task GenerateJwt(ApplicationUser applicationUser)
    {
        AccessToken = await _identityService.GenerateJwtAsync(applicationUser);
        await _localStorage.SetAsync(_tokenKey, AccessToken);
        _tenantProvider.TenantId   = applicationUser.TenantId;
        _tenantProvider.TenantName = applicationUser.TenantName;
        _currentUser.UserId        = applicationUser.Id;
        _currentUser.UserName      = applicationUser.UserName;
        _currentUser.TenantId      = applicationUser.TenantId;
        _currentUser.TenantName    = applicationUser.TenantName;
    }

    public async Task<ClaimsPrincipal> GetClaimsPrincipal()
    {
        try
        {
            ProtectedBrowserStorageResult<string> token = await _localStorage.GetAsync<string>(_tokenKey);
            if (token.Success && !string.IsNullOrEmpty(token.Value))
            {
                AccessToken = token.Value;
                ClaimsPrincipal? principal = await _identityService.GetClaimsPrincipal(token.Value);
                if (principal?.Identity?.IsAuthenticated ?? false)
                {
                    _tenantProvider.TenantId   = principal?.GetTenantId();
                    _tenantProvider.TenantName = principal?.GetTenantName();
                    _currentUser.UserId        = principal?.GetUserId();
                    _currentUser.UserName      = principal?.GetUserName();
                    _currentUser.TenantId      = principal?.GetTenantId();
                    _currentUser.TenantName    = principal?.GetTenantId();
                    return principal!;
                }
            }
        }
        catch (CryptographicException)
        {
            await RemoveAuthDataFromStorage();
        }
        catch (Exception)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new ClaimsPrincipal(new ClaimsIdentity());
    }

    public async Task RemoveAuthDataFromStorage()
    {
        await _localStorage.DeleteAsync(_tokenKey);
        _navigation.NavigateTo("/", true);
    }
}