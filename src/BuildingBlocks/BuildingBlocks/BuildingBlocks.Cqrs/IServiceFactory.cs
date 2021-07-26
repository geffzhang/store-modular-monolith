using System;

namespace BuildingBlocks.Cqrs
{
    public interface IServiceFactory
    {
        object GetInstance(Type T);
    }
}