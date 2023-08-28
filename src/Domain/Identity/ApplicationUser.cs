using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Blazor.Domain.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        UserClaims = new HashSet<ApplicationUserClaim>();
        UserRoles  = new HashSet<ApplicationUserRole>();
        Logins     = new HashSet<ApplicationUserLogin>();
        Tokens     = new HashSet<ApplicationUserToken>();
    }

    public string? DisplayName { get; set; }
    public string? Provider    { get; set; } = "Local";
    public string? TenantId    { get; set; }
    public string? TenantName  { get; set; }

    [Column(TypeName = "text")]
    public string? ProfilePictureDataUrl { get; set; }

    public bool                              IsActive               { get; set; }
    public bool                              IsLive                 { get; set; }
    public string?                           RefreshToken           { get; set; }
    public DateTime                          RefreshTokenExpiryTime { get; set; }
    public ICollection<ApplicationUserClaim> UserClaims             { get; set; }
    public ICollection<ApplicationUserRole>  UserRoles              { get; set; }
    public ICollection<ApplicationUserLogin> Logins                 { get; set; }
    public ICollection<ApplicationUserToken> Tokens                 { get; set; }

    public string? SuperiorId { get; set; } = null;

    [ForeignKey("SuperiorId")]
    public ApplicationUser? Superior { get; set; } = null;
}