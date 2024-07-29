namespace Plata.Models.Entities
{
    public partial class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BaseGrossSalary { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public ICollection<EmploymentContract> EmploymentContracts { get; set; } = [];
    }
}