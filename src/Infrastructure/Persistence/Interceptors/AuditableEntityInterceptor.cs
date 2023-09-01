using CleanArchitecture.Blazor.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
#nullable disable warnings
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService    _dateTimeService;
    private          List<AuditTrail>    _temporaryAuditTrailList = new List<AuditTrail>();

    public AuditableEntityInterceptor(ICurrentUserService currentUserService, IMediator mediator, IDateTimeService dateTimeServiceService)
    {
        _currentUserService = currentUserService;
        _dateTimeService    = dateTimeServiceService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context!);
        _temporaryAuditTrailList = TryInsertTemporaryAuditTrail(eventData.Context!, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        int resultValueTask = await base.SavedChangesAsync(eventData, result, cancellationToken);
        await TryUpdateTemporaryPropertiesForAuditTrail(eventData.Context!, cancellationToken)
            .ConfigureAwait(false);
        return resultValueTask;
    }

    private void UpdateEntities(DbContext context)
    {
        string? userId   = _currentUserService.UserId;
        foreach (EntityEntry<BaseAuditableEntity>? entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Created   = _dateTimeService.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModified   = _dateTimeService.Now;
                    break;
                
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedBy = userId;
                        softDelete.Deleted   = _dateTimeService.Now;
                        entry.State          = EntityState.Modified;
                    }
                    break;
                
                case EntityState.Unchanged:
                    if (entry.HasChangedOwnedEntities())
                    {
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModified   = _dateTimeService.Now;
                    }
                    break;
            }
        }
    }

    private List<AuditTrail> TryInsertTemporaryAuditTrail(DbContext context, CancellationToken cancellationToken = default)
    {
        string? userId   = _currentUserService.UserId;
        context.ChangeTracker.DetectChanges();
        List<AuditTrail>? temporaryAuditEntries = new List<AuditTrail>();
        foreach (EntityEntry<IAuditTrial>? entry in context.ChangeTracker.Entries<IAuditTrial>())
        {
            if (entry.Entity is AuditTrail || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged) //ToDo: Is AuditTrail check needed?
            {
                continue;
            }

            AuditTrail? auditEntry = new AuditTrail
                                     {
                                         TableName = entry.Entity.GetType()
                                                          .Name,
                                         UserId          = userId,
                                         DateTime        = _dateTimeService.Now,
                                         AffectedColumns = new List<string>(),
                                         NewValues       = new Dictionary<string, object?>(),
                                         OldValues       = new Dictionary<string, object?>()
                                     };
            foreach (PropertyEntry? property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey() && property.CurrentValue is not null)
                {
                    auditEntry.PrimaryKey[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        if (property.CurrentValue is not null)
                        {
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        if (property.OriginalValue is not null)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                        }

                        break;

                    case EntityState.Modified when property.IsModified &&
                                                   (property.OriginalValue is null && property.CurrentValue is not null || property.OriginalValue is not null && property.OriginalValue.Equals(property.CurrentValue) == false):
                        auditEntry.AffectedColumns.Add(propertyName);
                        auditEntry.AuditType               = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        if (property.CurrentValue is not null)
                        {
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;
                }
            }

            temporaryAuditEntries.Add(auditEntry);
        }

        return temporaryAuditEntries;
    }

    private async Task TryUpdateTemporaryPropertiesForAuditTrail(DbContext context, CancellationToken cancellationToken = default)
    {
        if (_temporaryAuditTrailList.Any())
        {
            foreach (AuditTrail? auditEntry in _temporaryAuditTrailList)
            {
                foreach (PropertyEntry? prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue is not null)
                    {
                        auditEntry.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (auditEntry.NewValues is not null && prop.CurrentValue is not null)
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
            }

            await context.AddRangeAsync(_temporaryAuditTrailList, cancellationToken);
            await context.SaveChangesAsync(cancellationToken)
                         .ConfigureAwait(false);
            _temporaryAuditTrailList.Clear();
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry.References.Any(r => r.TargetEntry != null && r.TargetEntry.Metadata.IsOwned() && (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}