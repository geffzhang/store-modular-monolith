using System.Threading;
using System.Threading.Tasks;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    public class NewUserRegisteredNotificationHandler : INotificationHandler<NewUserRegisteredNotification>
    {
        public async Task Handle(NewUserRegisteredNotification request, CancellationToken cancellationToken)
        {
            // Send email.
        }
    }
}