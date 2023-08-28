namespace CleanArchitecture.Blazor.Domain.Identity;

public sealed class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles  = new HashSet<ApplicationUserRole>();
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles  = new HashSet<ApplicationUserRole>();
    }

    public string?                           Description { get; set; }
    public ICollection<ApplicationRoleClaim> RoleClaims  { get; set; }
    public ICollection<ApplicationUserRole>  UserRoles   { get; set; }
}