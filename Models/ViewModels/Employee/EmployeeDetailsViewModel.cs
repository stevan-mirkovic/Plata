using Plata.Models.Interfaces;
using Plata.Models.Entities;


namespace Plata.Models.ViewModels
{
    public class EmployeePayslipsViewModel(Employee employee) : IViewModel
    {
        public Employee Employee { get; set; } = employee;
        public Payslip? NewPayslip { get; set; }
    }
}