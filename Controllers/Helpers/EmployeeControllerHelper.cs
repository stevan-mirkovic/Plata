using Microsoft.EntityFrameworkCore;
using Plata.Models.Entities;

namespace Plata.Controllers
{
    public partial class EmployeeController
    {
        private Employee? GetEmployeeForIndex(int id)
        {
            return dbContext.Employees
                .Where(e => e.Id == id && e.Company.UserAccountId == _authenticatedUserId)
                .Include(e => e.Address)
                .Include(e => e.EmploymentContracts)
                .ThenInclude(c => c.Position)
                .Include(e => e.EmploymentContracts)
                .ThenInclude(c => c.Payslips)
                .SingleOrDefault();
        }

        private Employee? GetEmployeeForEdit(int id)
        {
            return dbContext.Employees
                .Where(e => e.Id == id && e.Company.UserAccountId == _authenticatedUserId)
                .Include(e => e.Address)
                .Include(e => e.Company)
                .ThenInclude(c => c.Positions)
                .Include(e => e.EmploymentContracts)
                .ThenInclude(ct => ct.Position)
                .SingleOrDefault();
        }

        private Employee? GetEmployeeForDelete(int id)
        {
            return dbContext.Employees
                .Where(e => e.Id == id && e.Company.UserAccountId == _authenticatedUserId)
                .Include(e => e.Address)
                .Include(e => e.EmploymentContracts)
                .ThenInclude(ct => ct.Payslips)
                .SingleOrDefault();
        }

        private Employee? GetEmployeeForDeactivate(int id)
        {
            return dbContext.Employees
                .Where(e => e.Id == id && e.Company.UserAccountId == _authenticatedUserId)
                .Include(e => e.EmploymentContracts.Where(ct => ct.IsActive))
                .SingleOrDefault();
        }

        private Employee? GetEmployeeForPayslips(int id)
        {
            return dbContext.Employees
                .Where(e => e.Id == id && e.Company.UserAccountId == _authenticatedUserId)
                .Include(e => e.Company)
                .ThenInclude(c => c.PayPolicies.Where(pp => pp.IsActive))
                .Include(e => e.EmploymentContracts)
                .ThenInclude(c => c.Payslips)
                .SingleOrDefault();
        }

        private void EditEmployee(Employee employee, Employee editedEmployee)
        {
            EditEntity(employee, editedEmployee);
            EditEmployeeAddress(employee, editedEmployee);
            EditEmployeeActiveContract(employee, editedEmployee);
        }

        private void EditEmployeeAddress(Employee employee, Employee editedEmployee)
        {
            if (editedEmployee.Address != null)
            {
                if (employee.Address == null && !editedEmployee.Address.IsEmpty())
                {
                    employee.Address = editedEmployee.Address;
                    dbContext.SaveChanges();
                }

                else if (employee.Address != null && editedEmployee.Address.IsEmpty())
                {
                    dbContext.Remove(employee.Address);
                    dbContext.SaveChanges();
                }

                else if (employee.Address != null && !editedEmployee.Address.IsEmpty())
                {
                    employee.Address.Edit(editedEmployee.Address);
                    dbContext.SaveChanges();
                }
            }
        }
        private void EditEmployeeActiveContract(Employee employee, Employee editedEmployee)
        {
            var editedEmployeeActiveContract = editedEmployee.GetActiveContract();
            if (editedEmployeeActiveContract == null || editedEmployeeActiveContract.IsUnset()) return;
            var employeeActiveContract = employee.GetActiveContract();
            var matchedExistingContract = employee.GetMatchedContract(editedEmployeeActiveContract);

            if (matchedExistingContract == null)
            {
                employeeActiveContract?.Deactivate();
                dbContext.SaveChanges();
                employee.EmploymentContracts.Add(editedEmployeeActiveContract);
                dbContext.SaveChanges();
            }

            else if (matchedExistingContract != null && matchedExistingContract != employeeActiveContract)
            {
                employeeActiveContract?.Deactivate();
                dbContext.SaveChanges();
                matchedExistingContract.Activate();
                dbContext.SaveChanges();
            }
        }

        private void DeleteEmployee(Employee employee)
        {
            if (employee.Address != null) DeleteEntity(employee.Address);
            foreach (var payslip in employee.GetAllPayslips()) DeleteEntity(payslip);
            DeleteEntity(employee);
        }

        private void DeactivateEmployee(Employee employee)
        {
            employee.GetActiveContract()?.Deactivate();
            dbContext.SaveChanges();
        }
    }
}