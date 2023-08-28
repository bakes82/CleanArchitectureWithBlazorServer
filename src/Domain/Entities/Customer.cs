namespace CleanArchitecture.Blazor.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public string? Name        { get; set; }
    public string? Description { get; set; }
}