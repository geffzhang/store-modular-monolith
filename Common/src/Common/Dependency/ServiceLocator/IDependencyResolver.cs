using System;

namespace Common.Dependency
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