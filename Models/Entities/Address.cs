namespace Plata.Models.Entities
{
    public partial class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Company? Company { get; set; }
        public int? CompanyId { get; set; }
        public Employee? Employee { get; set; }
        public int? EmployeeId { get; set; }
    }
}