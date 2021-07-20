using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Common.Core.Domain;
using Common.Core.Domain.Types;
using Common.Core.Messaging.Outbox;
using Common.Persistence.MSSQL;
using Common.Persistence.MSSQL.Configurations;
using EntityFramework.Exceptions.SqlServer;
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
    public sealed class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
            IdentityUserClaim<string>,
            ApplicationUserRole,
            IdentityUserLogin<string>,
            IdentityRoleClaim<string>,
            IdentityUserToken<string>>,
        ISqlDbContext
    {
        private IDbContextTransaction _currentTransaction;

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //https://github.com/Giorgi/EntityFramework.Exceptions
            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());

            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model#add-navigation-properties
            builder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
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
            builder.Entity<ApplicationUserRole>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<ApplicationUserRole>().Property(x => x.RoleId).HasColumnName("RoleCode").HasMaxLength(50);
            builder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasColumnName("RoleCode")
                .HasMaxLength(50);
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
            builder.Entity<ApplicationUserRole>(b => { b.ToTable("UserRoles"); }).HasDefaultSchema("identities");
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