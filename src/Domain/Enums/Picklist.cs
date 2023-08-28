using System.ComponentModel;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum Picklist
{
    [Description("Status")] Status,
    [Description("Unit")]   Unit,
    [Description("Brand")]  Brand
}