using System.Collections.Generic;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    public class UserAddress : ValueObject
    {
        private UserAddress()
        {
        }

        private UserAddress(string street, string city, string state, string country, string zipcode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }

        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }

        public static UserAddress CreateNew(string street, string city, string state, string country, string zipcode)
        {
            return new(street, city, state, country, zipcode);
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
}