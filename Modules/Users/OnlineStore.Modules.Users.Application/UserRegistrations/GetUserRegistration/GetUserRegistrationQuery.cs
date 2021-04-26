using System;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.GetUserRegistration
{
    public class GetUserRegistrationQuery : QueryBase<UserRegistrationDto>
    {
        public GetUserRegistrationQuery(Guid userRegistrationId)
        {
            UserRegistrationId = userRegistrationId;
        }

        public Guid UserRegistrationId { get; }
    }
}