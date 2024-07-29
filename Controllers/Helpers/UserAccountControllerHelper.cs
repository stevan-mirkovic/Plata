using Microsoft.EntityFrameworkCore;
using Plata.Models.Entities;

namespace Plata.Controllers
{
    public partial class UserAccountController
    {
        private UserAccount? GetUserAccountForLogIn(string identifier)
        {
            return dbContext.UserAccounts
                .SingleOrDefault(u => u.Username == identifier || u.Company.EmailAddress == identifier);
        }

        private UserAccount? GetUserAccountForDelete()
        {
            return dbContext.UserAccounts.
                Where(u => u.Id == _authenticatedUserId)
                .Include(u => u.Company)
                .ThenInclude(c => c.Address)
                .Include(u => u.Company)
                .ThenInclude(c => c.Employees)
                .ThenInclude(e => e.Address)
                .Include(u => u.Company)
                .ThenInclude(c => c.Employees)
                .ThenInclude(e => e.EmploymentContracts)
                .ThenInclude(ec => ec.Payslips)
                .SingleOrDefault();
        }

        private void DeleteUserAccount(UserAccount userAccount)
        {
            if (userAccount.Company.Address != null) DeleteEntity(userAccount.Company.Address);
            
            foreach (var employee in userAccount.Company.Employees)
            {
                if (employee.Address != null) DeleteEntity(employee.Address);
                foreach (var payslip in employee.GetAllPayslips()) DeleteEntity(payslip);
            }

            DeleteEntity(userAccount);
        }
    }
}