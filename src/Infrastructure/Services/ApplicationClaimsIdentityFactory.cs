using CleanArchitecture.Blazor.Application.Constants.ClaimTypes;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
#nullable disable
public class ApplicationClaimsIdentityFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationClaimsIdentityFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        ClaimsPrincipal principal = await base.CreateAsync(user);
        if (!string.IsNullOrEmpty(user.TenantId))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
                                                            {
                                                                new Claim(ApplicationClaimTypes.TenantId, user.TenantId)
                                                            });
        }

        if (!string.IsNullOrEmpty(user.TenantName))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
                                                            {
                                                                new Claim(ApplicationClaimTypes.TenantName, user.TenantName)
                                                            });
        }

        if (!string.IsNullOrEmpty(user.SuperiorId))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
                                                            {
                                                                new Claim(ApplicationClaimTypes.SuperiorId, user.SuperiorId)
                                                            });
        }

        if (!string.IsNullOrEmpty(user.DisplayName))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
                                                            {
                                                                new Claim(ClaimTypes.GivenName, user.DisplayName)
                                                            });
        }

        if (!string.IsNullOrEmpty(user.ProfilePictureDataUrl))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
                                                            {
                                                                new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl)
                                                            });
        }

        ApplicationUser appuser = await _userManager.FindByIdAsync(user.Id);
        IList<string>   roles   = await _userManager.GetRolesAsync(appuser);
        if (roles != null && roles.Count > 0)
        {
            string rolesStr = string.Join(",", roles);
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
                                                            {
                                                                new Claim(ApplicationClaimTypes.AssignedRoles, rolesStr)
                                                            });
        }

        return principal;
    }
}