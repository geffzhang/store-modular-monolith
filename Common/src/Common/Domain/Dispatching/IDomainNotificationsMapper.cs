using System;

namespace Common.Domain.Dispatching
{
    public interface IDomainNotificationsMapper
    {
        string? GetName(Type type);
        Type? GetType(string name);
    }
}