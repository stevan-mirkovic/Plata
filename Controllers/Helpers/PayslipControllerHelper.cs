using Microsoft.EntityFrameworkCore;
using Plata.Models.Entities;

namespace Plata.Controllers
{
    public partial class PayslipController
    {
        public Payslip? GetPayslipForIndex(int id)
        {
            return dbContext.Payslips
                .Where(ps => ps.Id == id && ps.PayPolicy.Company.UserAccountId == _authenticatedUserId)
                .Include(ps => ps.EmploymentContract)
                .ThenInclude(ec => ec.Position)
                .Include(ps => ps.PayPolicy)
                .Include(ps => ps.EmploymentContract)
                .ThenInclude(ec => ec.Employee)
                .SingleOrDefault();
        }

        public Payslip? GetPayslipForCreate(int id)
        {
            return dbContext.Payslips
                .Where(ps => ps.Id == id && ps.PayPolicy.Company.UserAccountId == _authenticatedUserId)
                .Include(ps => ps.PayPolicy)
                .Include(ps => ps.EmploymentContract)
                .ThenInclude(ec => ec.Position)
                .SingleOrDefault();
        }

        public Payslip? GetPayslipForDelete(int id)
        {
            return dbContext.Payslips
                .Where(ps => ps.Id == id && ps.PayPolicy.Company.UserAccountId == _authenticatedUserId)
                .Include(ps => ps.EmploymentContract)
                .SingleOrDefault();
        }

        public void InitializePayslip(Payslip payslip)
        {
            payslip.Initialize();
            dbContext.SaveChanges();
        }

        public Payslip? GetPayslipForDownloadPdf(int id)
        {
            return dbContext.Payslips
                .Where(ps => ps.Id == id && ps.PayPolicy.Company.UserAccountId == _authenticatedUserId)
                .Include(ps => ps.PayPolicy)
                .ThenInclude(pp => pp.Company)
                .ThenInclude(c => c.Address)
                .Include(ps => ps.EmploymentContract)
                .ThenInclude(ec => ec.Employee)
                .ThenInclude(e => e.Address)
                .Include(ps => ps.EmploymentContract)
                .ThenInclude(ec => ec.Position)
                .SingleOrDefault();
        }
    }
}
