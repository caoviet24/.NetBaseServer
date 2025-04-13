using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptor
{
    public class AuditableInterceptor : SaveChangesInterceptor
    {
        private readonly IUser _user;
        private readonly TimeProvider _timeProvider;

        public AuditableInterceptor(
            IUser user,
            TimeProvider timeProvider)
        {
            _user = user;
            _timeProvider = timeProvider;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(Microsoft.EntityFrameworkCore.DbContext? context)
        {
            if (context == null) return;

            var currentUser = _user.getCurrentUser();

            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.createdBy = currentUser;
                    entry.Entity.createdAt = DateTime.UtcNow;
                    entry.Entity.isDeleted = false;
                }

                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity.isDeleted == true)
                    {
                        entry.Entity.isDeleted = true;
                        entry.Entity.deletedBy = currentUser;
                        entry.Entity.deletedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        entry.Entity.updatedBy = currentUser;
                        entry.Entity.updatedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}