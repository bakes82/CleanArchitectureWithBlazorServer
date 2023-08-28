using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Document : OwnerPropertyEntity, IMayHaveTenant, IAuditTrial
{
    public         string?      Title        { get; set; }
    public         string?      Description  { get; set; }
    public         JobStatus    Status       { get; set; } = default!;
    public         string?      Content      { get; set; }
    public         bool         IsPublic     { get; set; }
    public         string?      Url          { get; set; }
    public         DocumentType DocumentType { get; set; } = default!;
    public virtual Tenant?      Tenant       { get; set; }
    public         string?      TenantId     { get; set; }
}