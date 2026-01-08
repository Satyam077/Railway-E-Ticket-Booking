using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Domain.ValueObjects
{
    public class Address
    {
        [StringLength(200)]
        public string Street { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(10)]
        public string PinCode { get; set; }

        [StringLength(50)]
        public string Country { get; set; } = "India";

        public Address() { }

        public Address(string street, string city, string state, string pinCode, string country = "India")
        {
            Street = street;
            City = city;
            State = state;
            PinCode = pinCode;
            Country = country;
        }

        public string FullAddress => $"{Street}, {City}, {State} - {PinCode}, {Country}";

        public override string ToString() => FullAddress;

        public override bool Equals(object obj)
        {
            if (obj is not Address address) return false;
            
            return Street == address.Street &&
                   City == address.City &&
                   State == address.State &&
                   PinCode == address.PinCode &&
                   Country == address.Country;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street, City, State, PinCode, Country);
        }
    }
}
