using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Modules.Core.Domain.Domain.Entities
{
    public class Address : EntityBase<Guid>
    {
        public Address()
        {
        }

        public Address(long id)
        {
            Id = id;
        }

        [StringLength(450)] public string ContactName { get; set; }

        [StringLength(450)] public string Phone { get; set; }

        [StringLength(450)] public string AddressLine1 { get; set; }

        [StringLength(450)] public string AddressLine2 { get; set; }

        [StringLength(450)] public string City { get; set; }

        [StringLength(450)] public string ZipCode { get; set; }

        public long? DistrictId { get; set; }

        public District District { get; set; }

        public long StateOrProvinceId { get; set; }

        public StateOrProvince StateOrProvince { get; set; }

        public string CountryId { get; set; }

        public Country Country { get; set; }
    }
}