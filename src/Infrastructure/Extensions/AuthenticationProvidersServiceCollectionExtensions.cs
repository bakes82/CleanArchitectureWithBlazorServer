using CleanArchitecture.Blazor.Application.Common.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

/// <summary>
///     Extensions for configuring login via third party authentication providers.
/// </summary>
public static class AuthenticationProvidersServiceCollectionExtensions
{
    /// <summary>
    ///     Try to configure Microsoft account login if the application configuration has ClientId and ClientSecret.
    /// </summary>
    /// <param name="authenticationBuilder">Authentication Builder.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>
    ///     Authentication builder configured to sign in with a Microsoft account,
    ///     if the required keys are specified in the application configuration.
    /// </returns>
    public static AuthenticationBuilder TryConfigureMicrosoftAccount(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
    {
        var     authenticationSettings       = configuration.GetSection(AuthenticationSettings.Key).Get<AuthenticationSettings>();
        string? microsoftAccountClientId     = authenticationSettings?.Microsoft.ClientId;
        string? microsoftAccountClientSecret = authenticationSettings?.Microsoft.ClientSecret;
        
        if (authenticationSettings != null && (!authenticationSettings.Microsoft.Enabled || string.IsNullOrWhiteSpace(microsoftAccountClientId) || string.IsNullOrWhiteSpace(microsoftAccountClientSecret)))
        {
            return authenticationBuilder;
        }

        return authenticationBuilder.AddMicrosoftAccount(options =>
                                                         {
                                                             options.ClientId              = microsoftAccountClientId;
                                                             options.ClientSecret          = microsoftAccountClientSecret;
                                                             options.AccessDeniedPath      = "/pages/authentication/login";
                                                         });
    }

    /// <summary>
    ///     Try to configure Google account login if the application configuration has ClientId and ClientSecret.
    /// </summary>
    /// <param name="authenticationBuilder">Authentication Builder.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>
    ///     Authentication builder configured for Google account login,
    ///     if the required keys are specified in the application configuration.
    /// </returns>
    public static AuthenticationBuilder TryConfigureGoogleAccount(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
    {
        var     authenticationSettings    = configuration.GetSection(AuthenticationSettings.Key).Get<AuthenticationSettings>();
        string? googleAccountClientId     = authenticationSettings?.Google.ClientId;
        string? googleAccountClientSecret = authenticationSettings?.Google.ClientSecret;
        if (authenticationSettings != null && (!authenticationSettings.Google.Enabled || string.IsNullOrWhiteSpace(googleAccountClientId) || string.IsNullOrWhiteSpace(googleAccountClientSecret)))
        {
            return authenticationBuilder;
        }

        return authenticationBuilder.AddGoogle(options =>
                                               {
                                                   options.ClientId         = googleAccountClientId;
                                                   options.ClientSecret     = googleAccountClientSecret;
                                                   options.SignInScheme     = IdentityConstants.ExternalScheme;
                                                   options.AccessDeniedPath = "/pages/authentication/login";
                                                   options.Events = new OAuthEvents
                                                                    {
                                                                        OnCreatingTicket = c =>
                                                                                           {
                                                                                               ClaimsIdentity? identity = (ClaimsIdentity?)c?.Principal?.Identity;
                                                                                               string? avatar = c?.User.GetProperty("picture")
                                                                                                                 .GetString();
                                                                                               if (!string.IsNullOrEmpty(avatar))
                                                                                               {
                                                                                                   identity?.AddClaim(new Claim("avatar", avatar));
                                                                                               }

                                                                                               return Task.CompletedTask;
                                                                                           }
                                                                    };
                                               });
    }
}