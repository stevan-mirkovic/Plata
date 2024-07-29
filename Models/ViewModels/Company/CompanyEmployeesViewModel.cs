using Plata.Models.Interfaces;
using Plata.Models.Entities;

namespace Plata.Models.ViewModels
{
    public class CompanyEmployeesViewModel(Company company) : IViewModel
    {
        public Company Company { get; set; } = company;
        public Employee? NewEmployee { get; set; }
        public EmploymentContract? NewEmployeeContract { get; set; }
    }
}