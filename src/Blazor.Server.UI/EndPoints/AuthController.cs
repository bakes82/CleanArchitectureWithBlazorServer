using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Blazor.Server.UI.EndPoints;

public class AuthController : Controller
{
    private readonly IDataProtectionProvider        _dataProtectionProvider;
    private readonly ILogger<AuthController>        _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser>   _userManager;

    public AuthController(ILogger<AuthController> logger, IDataProtectionProvider dataProtectionProvider, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _logger                 = logger;
        _dataProtectionProvider = dataProtectionProvider;
        _userManager            = userManager;
        _signInManager          = signInManager;
    }

    /*[HttpGet("/auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string token, string returnUrl)
    {
        IDataProtector   dataProtector = _dataProtectionProvider.CreateProtector("Login");
        string           data          = dataProtector.Unprotect(token);
        string[]         parts         = data.Split('|');
        ApplicationUser? identityUser  = await _userManager.FindByIdAsync(parts[0]);
        if (identityUser == null)
        {
            return Unauthorized();
        }

        bool isTokenValid = await _userManager.VerifyUserTokenAsync(identityUser, TokenOptions.DefaultProvider, "Login", parts[1]);
        if (isTokenValid)
        {
            bool isPersistent = true;
            await _userManager.ResetAccessFailedCountAsync(identityUser);
            await _signInManager.SignInAsync(identityUser, isPersistent);
            identityUser.IsLive = true;
            await _userManager.UpdateAsync(identityUser);
            _logger.LogInformation("{@UserName} has successfully logged in", identityUser.UserName);
            return Redirect($"/{returnUrl}");
        }

        return Unauthorized();
    }*/

    /*[HttpGet("/auth/externallogin")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLogin(string provider, string userName, string name, string accessToken)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(userName);
        if (user is null)
        {
            ApplicationUser admin = await _userManager.FindByNameAsync("administrator") ?? throw new NotFoundException("Application user administrator Not Found.");
            user = new ApplicationUser
                   {
                       EmailConfirmed = true,
                       IsActive       = true,
                       IsLive         = true,
                       UserName       = userName,
                       Email          = userName.Any(x => x == '@') ? userName : $"{userName}@{provider}.com",
                       Provider       = provider,
                       DisplayName    = name,
                       SuperiorId     = admin.Id,
                       TenantId       = admin.TenantId,
                       TenantName     = admin.TenantName
                   };
            IdentityResult createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                return Unauthorized();
            }

            IdentityResult assignResult = await _userManager.AddToRoleAsync(user, RoleName.Basic);
            if (!assignResult.Succeeded)
            {
                return Unauthorized();
            }

            await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userName, accessToken));
        }

        if (!user.IsActive)
        {
            return Unauthorized();
        }

        await _signInManager.SignInAsync(user, true);
        return Redirect("/");
    }*/

    [HttpPost("/auth/externallogin")]
    public IActionResult ExternalLogin(string provider)
    {
        // Redirect to external provider
        string?                  redirectUrl = Url.Action("ExternalLoginCallback", "Auth");
        AuthenticationProperties properties  = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet("/auth/externallogincallback")]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            return Redirect("/pages/authentication/login");
        }

        ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return Redirect("/pages/authentication/Login");
        }

        SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);

        if (result.Succeeded)
        {
            return Redirect(returnUrl ?? "/"); // Change to your desired redirect
        }
        else
        {
            // If the user does not have an account, then you might need to redirect them 
            // to a Blazor page where they can confirm the account creation or provide additional details.
            return RedirectToPage("/ExternalLoginConfirmation");
        }
    }

    [HttpGet("/auth/logout")]
    public async Task<IActionResult> Logout()
    {
        string?         userId       = _signInManager.Context.User.GetUserId();
        ApplicationUser identityUser = await _userManager.FindByIdAsync(userId!) ?? throw new NotFoundException("Application user not found.");
        identityUser.IsLive = false;
        await _userManager.UpdateAsync(identityUser);
        _logger.LogInformation("{@UserName} logout successful", identityUser.UserName);
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}