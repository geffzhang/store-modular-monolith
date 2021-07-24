using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core.Domain.Types;
using EntityFrameworkCore.Triggered;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Services;

namespace OnlineStore.Modules.Identity.Infrastructure.Triggers
{
    // https://github.com/koenbeuk/EntityFrameworkCore.Triggered
    public class AuditTrigger : IBeforeSaveTrigger<IAuditable>
    {
        private readonly IUserNameResolver _currentUserNameResolver;

        public AuditTrigger(IUserNameResolver currentUserNameResolver)
        {
            _currentUserNameResolver = currentUserNameResolver;
        }

        public Task BeforeSave(ITriggerContext<IAuditable> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                var currentTime = DateTime.UtcNow;
                var userName = _currentUserNameResolver?.GetCurrentUserName();

                context.Entity.CreatedDate =
                    context.Entity.CreatedDate == default ? currentTime : context.Entity.CreatedDate;
                context.Entity.CreatedBy ??= userName;
            }
            else if (context.ChangeType == ChangeType.Modified)
            {
                var currentTime = DateTime.UtcNow;
                var userName = _currentUserNameResolver?.GetCurrentUserName();

                context.Entity.ModifiedDate = currentTime;

                if (string.IsNullOrEmpty(userName) == false)
                {
                    context.Entity.ModifiedBy = userName;
                }
            }

            return Task.CompletedTask;
        }
    }
}