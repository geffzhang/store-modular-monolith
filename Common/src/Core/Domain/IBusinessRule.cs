namespace Common.Core.Domain
{
    public interface IBusinessRule
    {
        string Message { get; }
        bool IsBroken();
    }
}