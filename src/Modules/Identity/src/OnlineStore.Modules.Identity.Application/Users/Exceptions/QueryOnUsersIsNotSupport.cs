using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class QueryOnUsersIsNotSupport:AppException
    {
        public QueryOnUsersIsNotSupport() : base("Querying on users is disabled.")
        {
        }
    }
}