using Plata.Models.Interfaces;

namespace Plata.Models.Entities
{
    public partial class PayPolicy : IEntity
    {
        public void Edit(IEntity editEntity) => throw new NotImplementedException();

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is not PayPolicy payPolicy) throw new ArgumentException("The `obj` argument must be of `PayPolicy` type");

            return TaxFreeSalaryPortion == payPolicy.TaxFreeSalaryPortion &&
                TaxRate == payPolicy.TaxRate &&
                EmployeeContributions.PensionContributionRate == payPolicy.EmployeeContributions.PensionContributionRate &&
                EmployeeContributions.HealthInsuranceRate == payPolicy.EmployeeContributions.HealthInsuranceRate &&
                EmployeeContributions.UnemploymentInsuranceRate == payPolicy.EmployeeContributions.UnemploymentInsuranceRate &&
                CompanyContributions.PensionContributionRate == payPolicy.CompanyContributions.PensionContributionRate &&
                CompanyContributions.HealthInsuranceRate == payPolicy.CompanyContributions.HealthInsuranceRate &&
                CompanyContributions.UnemploymentInsuranceRate == payPolicy.CompanyContributions.UnemploymentInsuranceRate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TaxFreeSalaryPortion, TaxRate, EmployeeContributions.PensionContributionRate, EmployeeContributions.HealthInsuranceRate, EmployeeContributions.UnemploymentInsuranceRate, CompanyContributions.PensionContributionRate, CompanyContributions.HealthInsuranceRate, CompanyContributions.UnemploymentInsuranceRate);
        }
    }
}