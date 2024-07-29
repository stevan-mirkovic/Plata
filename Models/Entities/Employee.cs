namespace Plata.Models.Entities
{
    public partial class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateOnly Birthday { get; set; }
        public DateOnly HireDate { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public Address? Address { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public ICollection<EmploymentContract> EmploymentContracts { get; set; } = [];
    }
}