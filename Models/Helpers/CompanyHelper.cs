using ClosedXML.Excel;
using Plata.Models.Interfaces;

namespace Plata.Models.Entities
{
    public partial class Company : IEntity
    {
        public void Edit(IEntity editedEntity)
        {
            if (editedEntity is not Company editedCompany) throw new ArgumentException("The `editedEntity` argument must be of `Company` type");

            Name = editedCompany.Name;
            FoundationDate = editedCompany.FoundationDate;
            Sector = editedCompany.Sector;
            Description = editedCompany.Description;
            PhoneNumber = editedCompany.PhoneNumber;
            EmailAddress = editedCompany.EmailAddress;
        }

        public PayPolicy GetActivePayPolicy() => PayPolicies.Single(pp => pp.IsActive);

        public PayPolicy? GetMatchedPayPolicy(PayPolicy payPolicyToMatch)
        {
            foreach (var payPolicy in PayPolicies) if (payPolicy.Equals(payPolicyToMatch)) return payPolicy;
            return default;
        }
    }
}