using Plata.Models.Interfaces;

namespace Plata.Models.Entities
{
    public partial class Address : IEntity
    {
        public void Edit(IEntity editedEntity)
        {
            if (editedEntity is not Address editedAddress) throw new ArgumentException("The `editedEntity` argument must be of `Address` type");

            Street = editedAddress.Street;
            Number = editedAddress.Number;
            City = editedAddress.City;
            PostalCode = editedAddress.PostalCode;
            Country = editedAddress.Country;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Street) &&
               string.IsNullOrEmpty(Number) &&
               string.IsNullOrEmpty(City) &&
               string.IsNullOrEmpty(PostalCode) &&
               string.IsNullOrEmpty(Country);
        }

        public bool IsIncomplete() => IsAnyPorpertyEmpty() && IsAnyPropertyNotEmpty();

        private bool IsAnyPorpertyEmpty()
        {
            return string.IsNullOrEmpty(Street) ||
               string.IsNullOrEmpty(Number) ||
               string.IsNullOrEmpty(City) ||
               string.IsNullOrEmpty(PostalCode) ||
               string.IsNullOrEmpty(Country);
        }

        private bool IsAnyPropertyNotEmpty()
        {
            return !string.IsNullOrEmpty(Street) ||
               !string.IsNullOrEmpty(Number) ||
               !string.IsNullOrEmpty(City) ||
               !string.IsNullOrEmpty(PostalCode) ||
               !string.IsNullOrEmpty(Country);
        }

        public override string ToString() => IsEmpty() ? string.Empty : $"{Street} {Number}, {City} {PostalCode}, {Country}";
    }
}