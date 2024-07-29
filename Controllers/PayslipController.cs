using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plata.Data;
using Plata.Enums;
using Plata.Models.Entities;
using Plata.Services;
using System.Globalization;

namespace Plata.Controllers
{
    [Authorize]
    public partial class PayslipController(AuthManager authManager, LocalDbContext dbContext, ExchangeRateApi exchangeRate, FilesGenerator filesGenerator) 
        : BaseController(authManager, dbContext, exchangeRate)
    {
        public async Task<IActionResult> Index(int id)
        {
            var currentPayslip = GetPayslipForIndex(id);
            if (currentPayslip == null) return NotFound();

            await SetDefaultViewBag();
            return View(currentPayslip);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Payslip newPayslip)
        {
            var currentCurrency = Request.Cookies["CurrentCurrency"];
            newPayslip.TaxableBonuses = await exchangeRate.ConvertToBaseCurrency(newPayslip.TaxableBonuses, currentCurrency);
            newPayslip.TaxFreeBonuses = await exchangeRate.ConvertToBaseCurrency(newPayslip.TaxFreeBonuses, currentCurrency);
            AddEntity(newPayslip);

            var createdPayslip = GetPayslipForCreate(newPayslip.Id);
            if (createdPayslip == null) return NotFound(); 
            InitializePayslip(createdPayslip);

            TempData["AppStatus"] = AppStatus.CreatePayslipSuccess;
            return RedirectToAction("Index", new { id = newPayslip.Id });
        }

        public IActionResult Delete(int id)
        {
            var currentPayslip = GetPayslipForDelete(id);
            if (currentPayslip == null) return NotFound();

            DeleteEntity(currentPayslip);

            TempData["AppStatus"] = AppStatus.DeletePayslipSuccess;
            return RedirectToAction("Payslips", "Employee", new { id = currentPayslip.EmploymentContract.EmployeeId });
        }

        public IActionResult DownloadPdf(int id)
        {
            var currentPayslip = GetPayslipForDownloadPdf(id);
            if (currentPayslip == null) return NotFound();

            var fileContent = filesGenerator.ExportPayslipToPdf(currentPayslip);
            var contentType = "application/pdf";
            var fileCulture = CultureInfo.CurrentCulture.Name;
            var fileName = fileCulture == "en-US" ? "Payslip.pdf"
                : fileCulture == "sr-Latn-RS" ? "PlatniListic.pdf"
                : fileCulture == "fr-FR" ? "FicheDePaie.pdf" : "";

            return File(fileContent, contentType, fileName);
        }
    }
}
