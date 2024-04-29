using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Logger;

namespace TechBlog.Infrastructure.Identity.Configuration
{
    public class EntityFrameworkIdentityContext : IdentityDbContext<EntityFrameworkIdentityUser>
    {
        private readonly ILoggerManager _logger;

        public EntityFrameworkIdentityContext(DbContextOptions<EntityFrameworkIdentityContext> options, ILoggerManager logger) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
            _logger = logger;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(EntityFrameworkIdentityContext).Assembly);

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(300)");

            base.OnModelCreating(builder);
        }

        public async Task MigrateAsync()
        {
            if ((await Database.GetPendingMigrationsAsync()).Any())
                await Database.MigrateAsync();
        }

        public async Task TestConnectionAsync()
        {
            try
            {
                _ = await Database.ExecuteSqlRawAsync("SELECT 1");
            }
            catch (Exception ex)
            {
                _logger.LogException("Invalid database connection", LoggerManagerSeverity.CRITICAL, ex, ("context", nameof(EntityFrameworkIdentityContext)));
                Environment.Exit(2);
            }
        }
    }
}
