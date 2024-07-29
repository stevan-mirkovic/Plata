using Plata.Models.Interfaces;

namespace Plata.Models.Entities
{
    public partial class EmploymentContract : IEntity
    {
        public void Edit(IEntity editEntity) => throw new NotImplementedException();

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        public bool IsUnset() => PositionId == 0 || SalaryCoefficient == 0;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is not EmploymentContract contract) throw new ArgumentException("The `obj` argument must be of `EmploymentContract` type");
            return PositionId == contract.PositionId && SalaryCoefficient == contract.SalaryCoefficient;
        }

        public override int GetHashCode() => HashCode.Combine(PositionId, SalaryCoefficient);
    }
}
