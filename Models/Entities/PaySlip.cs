namespace Plata.Models.Entities
{
    public partial class Payslip
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string TimePeriod => $"{StartDate} - {EndDate}";
        public decimal TaxableBonuses { get; set; }
        public decimal TaxFreeBonuses { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal CompanyCost { get; set; }
        public decimal NetSalary { get; set; }
        public EmploymentContract EmploymentContract { get; set; }
        public int ContractId { get; set; }
        public PayPolicy PayPolicy { get; set; }
        public int PayPolicyId { get; set; }
    }
}