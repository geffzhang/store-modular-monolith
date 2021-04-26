using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Common.Domain.Types;
using Newtonsoft.Json;

namespace OnlineStore.Modules.Customers.Domain.Customers
{
    public class Customer : AggregateRoot<Guid, CustomerId>
    {
        public IList<string> PhoneNumbers { get; set; } = new List<string>();
        public string Email => Emails.OrderBy(x => x).FirstOrDefault();

        /// <summary>
        /// Returns the email address of the customer.
        /// </summary>
        public IList<string> Emails { get; set; } = new List<string>();

        public string Name { get; set; }
        public string MemberType { get; set; }
        public IList<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
        public IList<string> Phones { get; set; } = new List<string>();
        public IList<string> Groups { get; set; } = new List<string>();

        /// <summary>
        /// User groups such as VIP, Wholesaler etc
        /// </summary>
        public IList<string> UserGroups { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{MemberType} {Name}";
        }
    }
}