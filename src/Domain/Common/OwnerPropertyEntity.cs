using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Domain.Common;

public abstract class OwnerPropertyEntity : BaseAuditableEntity
{
    [ForeignKey("CreatedBy")]
    public virtual ApplicationUser? Owner { get; set; }

    [ForeignKey("LastModifiedBy")]
    public virtual ApplicationUser? Editor { get; set; }
}