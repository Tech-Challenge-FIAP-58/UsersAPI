using FCG.Core.Events;
using FCG.Core.Mediatr;
using FCG.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FCG.Infra.Context
{
    public class ApplicationDbContext(IMediatorHandler _mediatorHandler, DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<StoredEvent> StoredEvents { get; set; }


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>()
                .HaveColumnType("varchar(100)");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Event>();

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }


        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();

            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("CreatedAtUtc") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAtUtc").CurrentValue = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("CreatedAtUtc").IsModified = false;
                }
            }

            var domainEntities = ChangeTracker.Entries<EntityBase>()
                .Where(x => x.Entity.Notifications != null && x.Entity.Notifications.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notifications)
                .ToList();
            var affectedRows = await base.SaveChangesAsync(cancellationToken);

            if (domainEvents.Count > 0)
            {
                foreach (var entry in domainEntities)
                {
                    foreach (var domainEvent in entry.Entity.Notifications)
                    {
                        StoredEvents.Add(new StoredEvent(
                            entry.Entity.Id,
                            entry.Entity.GetType().Name,
                            domainEvent.GetType().Name,
                            JsonSerializer.Serialize((object)domainEvent)
                        ));
                    }
                }

                domainEntities.ForEach(e => e.Entity.ClearEvents());

                await base.SaveChangesAsync(cancellationToken);

                var tasks = domainEvents.Select(e => _mediatorHandler.PublishEvent(e));
                await Task.WhenAll(tasks);
            }

            return affectedRows;
        }
    }

    public static class MediatorExtension
    {
        public static async Task PublishEvents<T>(this IMediatorHandler mediator, T ctx) where T : DbContext
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.Notifications != null && x.Entity.Notifications.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notifications)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.PublishEvent(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }

}