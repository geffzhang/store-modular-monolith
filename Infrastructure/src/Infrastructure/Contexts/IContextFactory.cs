using IInfrastructure.Contexts;

namespace Infrastructure.Contexts
{
    internal interface IContextFactory
    {
        IContext Create();
    }
}