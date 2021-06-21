using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Types;
using Common.Messaging.Outbox;
using Common.Persistence.MSSQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications
    /// </summary>
    public sealed class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, ISqlDbContext, IDomainEventContext

    {
        private IDbContextTransaction _currentTransaction;

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserRole<string>>(userRole =>
            {
                userRole.HasOne<ApplicationRole>()
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);

                userRole.HasOne<ApplicationUser>()
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId);
            });

            builder.Entity<ApplicationUser>()
                .Ignore(x => x.Permissions); // this ignored properties will handle in UserManager and RoleManager
            builder.Entity<ApplicationRole>().Ignore(x => x.Permissions);
            builder.Entity<ApplicationUser>().Ignore(x => x.Roles);
            builder.Entity<ApplicationUser>().Ignore(x => x.Logins);
            builder.Entity<ApplicationUser>().Property(x => x.UserType).HasMaxLength(64);
            builder.Entity<ApplicationUser>().Property(x => x.PhotoUrl).HasMaxLength(2048);
            builder.Entity<ApplicationUser>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            builder.Entity<ApplicationRole>().Property(x => x.Id).HasColumnName("Code").HasMaxLength(50);
            builder.Entity<IdentityUserClaim<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);
            builder.Entity<IdentityUserRole<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserRole<string>>().Property(x => x.RoleId).HasColumnName("RoleCode").HasMaxLength(50);
            builder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasColumnName("RoleCode").HasMaxLength(50);
            builder.Entity<IdentityUserToken<string>>().Property(x => x.UserId).HasMaxLength(128);

            MapsTables(builder);
        }

        private static void MapsTables(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(b => { b.ToTable("User"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaim"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogin"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserToken"); }).HasDefaultSchema("identities");
            builder.Entity<ApplicationRole>(b => { b.ToTable("Role"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UserRoles"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaim"); }).HasDefaultSchema("identities");
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();

                await (_currentTransaction?.CommitAsync() ?? Task.CompletedTask);
            }
            catch
            {
                await RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransaction()
        {
            try
            {
                await _currentTransaction?.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            var domainEntities = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x =>
                    x.Entity.Events != null &&
                    x.Entity.Events.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Events)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.Events?.ToList().Clear());

            return domainEvents;
        }
    }
}