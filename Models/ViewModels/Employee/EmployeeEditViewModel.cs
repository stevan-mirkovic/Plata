using Plata.Models.Interfaces;
using Plata.Models.Entities;


namespace Plata.Models.ViewModels
{
    public class EmployeeEditViewModel(Employee employee) : IViewModel
    {
        public Employee EditedEmployee { get; set; } = employee;
        public EmploymentContract? EditedEmployeeContract { get; set; } = employee.GetActiveContract();
        public AuthenticationViewModel? Credentials { get; set; }
    }
}