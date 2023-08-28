using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class KeyValue : BaseAuditableEntity, IAuditTrial
{
    public Picklist Name        { get; set; } = Picklist.Brand;
    public string?  Value       { get; set; }
    public string?  Text        { get; set; }
    public string?  Description { get; set; }
}