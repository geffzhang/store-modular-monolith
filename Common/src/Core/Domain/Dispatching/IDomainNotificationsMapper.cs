using System;

namespace Common.Core.Domain.Dispatching
{
    public interface IDomainNotificationsMapper
    {
        string? GetName(Type type);
        Type? GetType(string name);
    }
}