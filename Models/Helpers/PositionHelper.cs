using Plata.Models.Interfaces;

namespace Plata.Models.Entities
{
    public partial class Position : IEntity
    {
        public void Edit(IEntity editedEntity)
        {
            if (editedEntity is not Position editedPosition) throw new ArgumentException("The `editedEntity` argument must be of `Position` type");

            Name = editedPosition.Name;
            BaseGrossSalary = editedPosition.BaseGrossSalary;
        }

        public List<EmploymentContract> GetActiveContracts() => EmploymentContracts.Where(ct => ct.IsActive).ToList();
    }
}
