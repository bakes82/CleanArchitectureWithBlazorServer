using CleanArchitecture.Blazor.Domain.Entities.Logger;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Logger>     Loggers       { get; set; }
    DbSet<AuditTrail> AuditTrails   { get; set; }
    DbSet<Tenant>     Tenants       { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}