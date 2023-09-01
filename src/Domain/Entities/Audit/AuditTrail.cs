using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleanArchitecture.Blazor.Domain.Entities.Audit;

public class AuditTrail : IEntity<Guid>
{
    public         string?                      UserId          { get; set; }
    public virtual ApplicationUser?             Owner           { get; set; }
    public         AuditType                    AuditType       { get; set; }
    public         string?                      TableName       { get; set; }
    public         DateTime                     DateTime        { get; set; }
    public         Dictionary<string, object?>? OldValues       { get; set; }
    public         Dictionary<string, object?>? NewValues       { get; set; }
    public         List<string>?                AffectedColumns { get; set; }
    public         Dictionary<string, object>   PrimaryKey      { get; set; } = new Dictionary<string, object>();

    public List<PropertyEntry> TemporaryProperties    { get; } = new List<PropertyEntry>();
    public bool                HasTemporaryProperties => TemporaryProperties.Any();
    public Guid                 Id                     { get; set; }
}