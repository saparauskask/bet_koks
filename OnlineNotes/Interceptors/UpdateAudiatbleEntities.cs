using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnlineNotes.Models.Interfaces;

namespace OnlineNotes.Interceptors
{
    public class UpdateAudiatbleEntities : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;

            if (dbContext == null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var entities = dbContext.ChangeTracker.Entries<IAuditable>();

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Property(x => x.CreationDate).CurrentValue = DateTime.Now;
                }

                if (entity.State == EntityState.Modified)
                {
                    entity.Property(x => x.ModificationDate).CurrentValue = DateTime.Now;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
