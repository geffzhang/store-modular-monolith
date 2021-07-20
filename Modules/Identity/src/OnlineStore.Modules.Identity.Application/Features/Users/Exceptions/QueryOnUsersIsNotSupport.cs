using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Exceptions
{
    public class QueryOnUsersIsNotSupport:AppException
    {
        public QueryOnUsersIsNotSupport() : base("Querying on users is disabled.")
        {
        }
    }
}