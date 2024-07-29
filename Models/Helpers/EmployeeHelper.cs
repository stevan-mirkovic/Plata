using Plata.Models.Interfaces;

namespace Plata.Models.Entities
{
    public partial class Employee : IEntity
    {
        public void Edit(IEntity editedEntity)
        {
            if (editedEntity is not Employee editedEmployee) throw new ArgumentException("The `editedEntity` argument must be of `Employee` type");

            FirstName = editedEmployee.FirstName;
            LastName = editedEmployee.LastName;
            Birthday = editedEmployee.Birthday;
            PhoneNumber = editedEmployee.PhoneNumber;
            EmailAddress = editedEmployee.EmailAddress;
        }
        
        public EmploymentContract? GetActiveContract() => EmploymentContracts.SingleOrDefault(c => c.IsActive);

        public bool HasActiveContract() => GetActiveContract() != null;

        public List<Payslip> GetAllPayslips()
        {
            var payslips = new List<Payslip>();
            foreach (var contract in EmploymentContracts) payslips.AddRange(contract.Payslips);
            return payslips;
        }

        public Payslip? GetLastPayslip() => GetAllPayslips().OrderByDescending(ps => ps.StartDate).LastOrDefault();

        public EmploymentContract? GetMatchedContract(EmploymentContract contractToMatch)
        {
            foreach (var contract in EmploymentContracts) if (contract.Equals(contractToMatch)) return contract;
            return default;
        }
    }
}
