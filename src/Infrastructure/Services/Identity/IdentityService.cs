using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly AppConfigurationSettings                     _appConfig;
    private readonly IAuthorizationService                        _authorizationService;
    private readonly IAppCache                                    _cache;
    private readonly IStringLocalizer<IdentityService>            _localizer;
    private readonly IMapper                                      _mapper;
    private readonly RoleManager<ApplicationRole>                 _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly UserManager<ApplicationUser>                 _userManager;

    public IdentityService(IServiceScopeFactory scopeFactory, AppConfigurationSettings appConfig, IAppCache cache, IMapper mapper, IStringLocalizer<IdentityService> localizer)
    {
        IServiceScope scope = scopeFactory.CreateScope();
        _userManager                = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager                = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _authorizationService       = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        _appConfig                  = appConfig;
        _cache                      = cache;
        _mapper                     = mapper;
        _localizer                  = localizer;
    }

    private TimeSpan              RefreshInterval => TimeSpan.FromSeconds(60);
    private LazyCacheEntryOptions Options         => new LazyCacheEntryOptions().SetAbsoluteExpiration(RefreshInterval, ExpirationMode.LazyExpiration);

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        string           key  = $"GetUserNameAsync:{userId}";
        ApplicationUser? user = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId), Options);
        return user?.UserName;
    }

    public string GetUserName(string userId)
    {
        string           key  = $"GetUserName-byId:{userId}";
        ApplicationUser? user = _cache.GetOrAdd(key, () => _userManager.Users.SingleOrDefault(u => u.Id == userId), Options);
        return user?.UserName ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        ApplicationUser user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException(_localizer["User Not Found."]);
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        ApplicationUser     user      = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException(_localizer["User Not Found."]);
        ClaimsPrincipal     principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        AuthorizationResult result    = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default)
    {
        ApplicationUser user   = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException(_localizer["User Not Found."]);
        IdentityResult  result = await _userManager.DeleteAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<IDictionary<string, string?>> FetchUsers(string roleName, CancellationToken cancellation = default)
    {
        Dictionary<string, string?> result = await _userManager.Users.Where(x => x.UserRoles.Any(y => y.Role.Name == roleName))
                                                               .Include(x => x.UserRoles)
                                                               .ToDictionaryAsync(x => x.UserName!, y => y.DisplayName, cancellation);
        return result;
    }

    public async Task<Result<TokenResponse>> LoginAsync(TokenRequest request, CancellationToken cancellation = default)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(request.UserName!);
        if (user == null)
        {
            return await Result<TokenResponse>.FailureAsync(new string[]
                                                            {
                                                                _localizer["User Not Found."]
                                                            });
        }

        if (!user.IsActive)
        {
            return await Result<TokenResponse>.FailureAsync(new string[]
                                                            {
                                                                _localizer["User Not Active. Please contact the administrator."]
                                                            });
        }

        if (!user.EmailConfirmed)
        {
            return await Result<TokenResponse>.FailureAsync(new string[]
                                                            {
                                                                _localizer["E-Mail not confirmed."]
                                                            });
        }

        bool passwordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
        if (!passwordValid)
        {
            return await Result<TokenResponse>.FailureAsync(new string[]
                                                            {
                                                                _localizer["Invalid Credentials."]
                                                            });
        }

        user.RefreshToken = GenerateRefreshToken();
        DateTime tokenExpiryTime = DateTime.Now.AddDays(7);

        if (request.RememberMe)
        {
            tokenExpiryTime = DateTime.Now.AddYears(1);
        }

        user.RefreshTokenExpiryTime = tokenExpiryTime;
        await _userManager.UpdateAsync(user);

        string        token    = await GenerateJwtAsync(user);
        TokenResponse response = new TokenResponse { Token = token, RefreshTokenExpiryTime = tokenExpiryTime, RefreshToken = user.RefreshToken, ProfilePictureDataUrl = user.ProfilePictureDataUrl };
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellation = default)
    {
        if (request is null)
        {
            return await Result<TokenResponse>.FailureAsync(new string[]
                                                            {
                                                                _localizer["Invalid Client Token."]
                                                            });
        }

        ClaimsPrincipal  userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string           userEmail     = userPrincipal.FindFirstValue(ClaimTypes.Email)!;
        ApplicationUser? user          = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return await Result<TokenResponse>.FailureAsync(new string[]
                                                            {
                                                                _localizer["User Not Found."]
                                                            });
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return await Result<TokenResponse>.FailureAsync(new string[]
                                                            {
                                                                _localizer["Invalid Client Token."]
                                                            });
        }

        ClaimsPrincipal principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        string          token     = GenerateEncryptedToken(GetSigningCredentials(), principal.Claims);
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        TokenResponse response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<ClaimsPrincipal> GetClaimsPrincipal(string token)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                                                              {
                                                                  ValidateIssuerSigningKey = false,
                                                                  IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
                                                                  ValidateIssuer           = false,
                                                                  ValidateAudience         = false,
                                                                  RoleClaimType            = ClaimTypes.Role,
                                                                  ClockSkew                = TimeSpan.Zero,
                                                                  ValidateLifetime         = true
                                                              };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        TokenValidationResult?  result       = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
        if (result.IsValid)
        {
            return new ClaimsPrincipal(result.ClaimsIdentity);
        }

        return new ClaimsPrincipal(new ClaimsIdentity());
    }

    public async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        ClaimsPrincipal principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        string          token     = GenerateEncryptedToken(GetSigningCredentials(), principal.Claims);
        return token;
    }

    public async Task UpdateLiveStatus(string userId, bool isLive, CancellationToken cancellation = default)
    {
        ApplicationUser? user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsLive != isLive, cancellation);
        if (user is not null)
        {
            user.IsLive = isLive;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task<ApplicationUserDto> GetApplicationUserDto(string userId, CancellationToken cancellation = default)
    {
        string key = $"GetApplicationUserDto:{userId}";
        ApplicationUserDto? result = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.Where(x => x.Id == userId)
                                                                                                   .Include(x => x.UserRoles)
                                                                                                   .ThenInclude(x => x.Role)
                                                                                                   .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                                                                                                   .FirstAsync(cancellation), Options);
        return result;
    }

    public async Task<List<ApplicationUserDto>?> GetUsers(string? tenantId, CancellationToken cancellation = default)
    {
        string key = $"GetApplicationUserDtoListWithTenantId:{tenantId}";
        Func<string?, CancellationToken, Task<List<ApplicationUserDto>?>> getUsersByTenantId = async (id, cancellationToken) =>
                                                                                               {
                                                                                                   if (string.IsNullOrEmpty(id))
                                                                                                   {
                                                                                                       return await _userManager.Users.Include(x => x.UserRoles)
                                                                                                                                .ThenInclude(x => x.Role)
                                                                                                                                .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                                                                                                                                .ToListAsync(cancellationToken);
                                                                                                   }

                                                                                                   return await _userManager.Users.Where(x => x.TenantId == id)
                                                                                                                            .Include(x => x.UserRoles)
                                                                                                                            .ThenInclude(x => x.Role)
                                                                                                                            .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                                                                                                                            .ToListAsync(cancellationToken);
                                                                                               };
        List<ApplicationUserDto>? result = await _cache.GetOrAddAsync(key, () => getUsersByTenantId(tenantId, cancellation), Options);
        return result;
    }

    private string GenerateRefreshToken()
    {
        byte[]                      randomNumber = new byte[32];
        using RandomNumberGenerator rng          = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        JwtSecurityToken        token          = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddDays(2), signingCredentials: signingCredentials);
        JwtSecurityTokenHandler tokenHandler   = new JwtSecurityTokenHandler();
        string?                 encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                                                              {
                                                                  ValidateIssuerSigningKey = true,
                                                                  IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
                                                                  ValidateIssuer           = false,
                                                                  ValidateAudience         = false,
                                                                  RoleClaimType            = ClaimTypes.Role,
                                                                  ClockSkew                = TimeSpan.Zero,
                                                                  ValidateLifetime         = false
                                                              };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal?        principal    = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken? securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException(_localizer["Invalid token"]);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}