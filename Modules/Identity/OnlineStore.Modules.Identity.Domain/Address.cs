using System.Collections.Generic;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain
{
    public class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }
        public string RegionId { get; set; }
        public string RegionName { get; set; }

        private Address()
        {
        }

        private Address(string street, string city, string state, string country, string zipcode,
            string regionId = null, string regionName = null)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
            RegionId = regionId;
            RegionName = regionName;
        }

        public static Address Of(string street, string city, string state, string country, string zipcode = null,
            string regionId = null, string regionName = null)
        {
            return new(street, city, state, country, zipcode, regionId, regionName);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
            yield return RegionId;
            yield return RegionName;
        }
    }
}