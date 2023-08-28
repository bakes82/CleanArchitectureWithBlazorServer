namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationUserLogin : IdentityUserLogin<string>
{
    public virtual ApplicationUser User { get; set; } = default!;
}