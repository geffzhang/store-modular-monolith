namespace Common.Contexts
{
    internal interface IContextFactory
    {
        IContext Create();
    }
}