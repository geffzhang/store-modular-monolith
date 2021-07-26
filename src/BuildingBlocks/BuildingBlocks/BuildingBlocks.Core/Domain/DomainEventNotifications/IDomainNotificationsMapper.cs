using System;

namespace BuildingBlocks.Core.Domain.DomainEventNotifications
{
    public interface IDomainNotificationsMapper
    {
        string? GetName(Type type);
        Type? GetType(string name);
    }
}