using System.Security.Cryptography;
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
    private readonly string                _tokenKey = nameof(_tokenKey);

    public AccessTokenProvider(ProtectedLocalStorage localStorage, NavigationManager navigation, IIdentityService identityService, ICurrentUserService currentUser)
    {
        _localStorage    = localStorage;
        _navigation      = navigation;
        _identityService = identityService;
        _currentUser     = currentUser;
    }

    public string? AccessToken { get; private set; }

    public async Task GenerateJwt(ApplicationUser applicationUser)
    {
        AccessToken = await _identityService.GenerateJwtAsync(applicationUser);
        await _localStorage.SetAsync(_tokenKey, AccessToken);
        _currentUser.UserId        = applicationUser.Id;
        _currentUser.UserName      = applicationUser.UserName;
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
                    _currentUser.UserId        = principal?.GetUserId();
                    _currentUser.UserName      = principal?.GetUserName(); return principal!;
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