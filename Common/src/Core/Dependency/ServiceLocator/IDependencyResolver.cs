using System;

namespace Common.Core.Dependency.ServiceLocator
{
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolves the message handles or consumers.
        /// </summary>
        /// <returns></returns>
        object Resolve(Type type);
    }
}