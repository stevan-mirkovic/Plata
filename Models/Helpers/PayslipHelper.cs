using Plata.Models.Interfaces;

namespace Plata.Models.Entities
{
    public partial class Payslip: IEntity
    {
        public void Edit(IEntity entity) => throw new NotImplementedException();

        public void Initialize()
        {
            GrossSalary = GetGrossSalary();
            CompanyCost = GetCompanyCost();
            NetSalary = GetNetSalary();
        }

        private decimal GetGrossSalary()
        {
            var baseSalary = EmploymentContract.Position.BaseGrossSalary;
            var coefficient = EmploymentContract.SalaryCoefficient;
            return baseSalary * coefficient + TaxableBonuses + TaxFreeBonuses;
        }

        private decimal GetCompanyCost()
        {
            var contributionsBase = GrossSalary - TaxFreeBonuses;
            var pensionContribution = contributionsBase * PayPolicy.CompanyContributions.PensionContributionRate;
            var healthInsurance = contributionsBase * PayPolicy.CompanyContributions.HealthInsuranceRate;
            var unemploymentInsurance = contributionsBase * PayPolicy.CompanyContributions.UnemploymentInsuranceRate;
            return GrossSalary + pensionContribution + healthInsurance + unemploymentInsurance;
        }

        private decimal GetNetSalary()
        {
            var taxableSalaryPortion = GrossSalary - PayPolicy.TaxFreeSalaryPortion - TaxFreeBonuses;
            var tax = taxableSalaryPortion * PayPolicy.TaxRate;
            var contributionsBase = GrossSalary - TaxFreeBonuses;
            var pensionContribution = contributionsBase * PayPolicy.EmployeeContributions.PensionContributionRate;
            var healthInsurance = contributionsBase * PayPolicy.EmployeeContributions.HealthInsuranceRate;
            var unemploymentInsurance = contributionsBase * PayPolicy.EmployeeContributions.UnemploymentInsuranceRate;
            return GrossSalary - tax - pensionContribution - healthInsurance - unemploymentInsurance;
        }
    }
}