namespace Plata.Models.Entities
{
    public partial class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly FoundationDate { get; set; }
        public string Sector { get; set; }
        public string? Description { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public Address? Address { get; set; }
        public UserAccount UserAccount { get; set; }
        public int UserAccountId { get; set; }
        public ICollection<PayPolicy> PayPolicies { get; set; } = [];
        public ICollection<Position> Positions { get; set; } = [];
        public ICollection<Employee> Employees { get; set; } = [];
    }
}