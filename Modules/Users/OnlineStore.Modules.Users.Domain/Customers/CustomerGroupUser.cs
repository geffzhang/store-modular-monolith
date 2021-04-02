using OnlineStore.Modules.Users.Domain.Users;

namespace OnlineStore.Modules.Users.Domain.Customers
{
    public class CustomerGroupUser
    {
        public long UserId { get; set; }

        public User User { get; set; }

        public long CustomerGroupId { get; set; }

        public CustomerGroup CustomerGroup { get; set; }
    }
}