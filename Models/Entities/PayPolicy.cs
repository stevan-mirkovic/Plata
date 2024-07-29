using Plata.Models.OwnedEntities;

namespace Plata.Models.Entities
{
    public partial class PayPolicy
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal TaxFreeSalaryPortion { get; set; } = 25000;
        public decimal TaxRate { get; set; } = 0.1m;
        public ContributionsPolicy EmployeeContributions { get; set; } = new (pension: 0.14m, health: 0.0515m, unemployment: 0.075m);
        public ContributionsPolicy CompanyContributions { get; set; } = new (pension: 0.14m, health: 0.0515m, unemployment: 0);
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public ICollection<Payslip> Payslips { get; set; } = [];
    }
}