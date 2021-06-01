namespace Common.Web.Contexts
{
    internal interface IContextFactory
    {
        IContext Create();
    }
}