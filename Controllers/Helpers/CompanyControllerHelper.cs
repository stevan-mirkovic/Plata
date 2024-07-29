using Microsoft.EntityFrameworkCore;
using Plata.Models.Entities;

namespace Plata.Controllers
{
    public partial class CompanyController
    {
        private Company? GetCompanyForIndex()
        {
            return dbContext.Companies
                .Where(c => c.UserAccountId == _authenticatedUserId)
                .Include(c => c.Address)
                .Include(c => c.Employees)
                .ThenInclude(e => e.EmploymentContracts)
                .ThenInclude(ct => ct.Position)
                .Include(c => c.Positions)
                .ThenInclude(p => p.EmploymentContracts)
                .Include(c => c.PayPolicies.Where(pp => pp.IsActive))
                .SingleOrDefault();
        }

        private Company? GetCompanyForEdit()
        {
            return dbContext.Companies
                .Where(c => c.UserAccountId == _authenticatedUserId)
                .Include(c => c.PayPolicies)
                .Include(c => c.Address)
                .SingleOrDefault();
        }

        private Company? GetCompanyForPositions()
        {
            return dbContext.Companies
                .Where(c => c.UserAccountId == _authenticatedUserId)
                .Include(c => c.Positions)
                .ThenInclude(p => p.EmploymentContracts)
                .SingleOrDefault();
        }

        private Company? GetCompanyForEmployees()
        {
            return dbContext.Companies
                .Where(c => c.UserAccountId == _authenticatedUserId)
                .Include(c => c.Employees)
                .ThenInclude(e => e.EmploymentContracts.Where(ct => ct.IsActive))
                .ThenInclude(ct => ct.Position)
                .Include(c => c.Positions)
                .SingleOrDefault();
        }

        private void EditCompany(Company company, Company editedCompany)
        {
            EditEntity(company, editedCompany);
            EditCompanyAddress(company, editedCompany);
            EditCompanyActivePayPolicy(company, editedCompany);
        }

        private void EditCompanyAddress(Company company, Company editedCompany)
        {
            if (editedCompany.Address != null)
            {
                if (company.Address == null && !editedCompany.Address.IsEmpty())
                {
                    company.Address = editedCompany.Address;
                    dbContext.SaveChanges();
                }

                else if (company.Address != null && editedCompany.Address.IsEmpty())
                {
                    DeleteEntity(company.Address);
                }

                else if (company.Address != null && !editedCompany.Address.IsEmpty())
                {
                    EditEntity(company.Address, editedCompany.Address);
                }
            }
        }

        private void EditCompanyActivePayPolicy(Company company, Company editedCompany)
        {
            var companyActivePayPolicy = company.GetActivePayPolicy();
            var editedCompanyActivePayPolicy = editedCompany.GetActivePayPolicy();
            var matchedPayPolicy = company.GetMatchedPayPolicy(editedCompanyActivePayPolicy);

            if (matchedPayPolicy == null)
            {
                companyActivePayPolicy.Deactivate();
                dbContext.SaveChanges();
                company.PayPolicies.Add(editedCompanyActivePayPolicy);
                dbContext.SaveChanges();
            }

            else if (matchedPayPolicy != null && matchedPayPolicy != companyActivePayPolicy)
            {
                companyActivePayPolicy.Deactivate();
                dbContext.SaveChanges();
                matchedPayPolicy.Activate();
                dbContext.SaveChanges();
            }
        }

        public async Task InitializeCompanyPayPolicy(Company company, PayPolicy payPolicy)
        {
            payPolicy.TaxFreeSalaryPortion = await exchangeRate.ConvertToBaseCurrency(payPolicy.TaxFreeSalaryPortion, Request.Cookies["CurrentCurrency"]);
            company.PayPolicies.Add(payPolicy);
        }


        public Company? GetCompanyForEmployeesDownload()
        {
            return dbContext.Companies
                .Where(c => c.UserAccountId == _authenticatedUserId)
                .Include(c => c.Employees)
                .ThenInclude(e => e.Address)
                .Include(c => c.Employees)
                .ThenInclude(e => e.EmploymentContracts)
                .ThenInclude(ec => ec.Payslips)
                .SingleOrDefault();
        }
    }
}