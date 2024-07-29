namespace Plata.Models.Entities
{
    public partial class EmploymentContract
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal SalaryCoefficient { get; set; }
        public Position Position { get; set; }
        public int PositionId { get; set; }
        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
        public ICollection<Payslip> Payslips { get; set; } = [];
    }
}