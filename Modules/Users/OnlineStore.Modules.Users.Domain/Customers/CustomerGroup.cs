using System.Collections.Generic;

namespace OnlineStore.Modules.Users.Domain.Customers
{
    public class CustomerGroup : AggregateRoot<CustomerGroupId>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public IList<CustomerGroupUser> Users { get; set; } = new List<CustomerGroupUser>();
    }
}