using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plata.Enums;
using Plata.Data;
using Plata.Models.ViewModels;
using Plata.Models.Entities;
using Plata.Services;
using System.Globalization;

namespace Plata.Controllers
{
    [Authorize]
    public partial class CompanyController(AuthManager authManager, LocalDbContext dbContext, ExchangeRateApi exchangeRate, FilesGenerator filesGenerator) 
        : BaseController(authManager, dbContext, exchangeRate)
    {
        public async Task<IActionResult> Index()
        {
            var currentCompany = GetCompanyForIndex();
            if (currentCompany == null) return NotFound();

            await SetDefaultViewBag();
            return View(currentCompany);
        }

        public async Task<IActionResult> Edit()
        {
            var currentCompany = GetCompanyForEdit();
            if (currentCompany == null) return NotFound();

            await SetDefaultViewBag();
            return View(new CompanyEditViewModel(currentCompany));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Company editedCompany, PayPolicy editedCompanyPayPolicy, AuthenticationViewModel credentials)
        {
            if (credentials.ConfirmationPassword == null)
            {
                TempData["AppStatus"] = AppStatus.NoConfirmationPasswordFail;
                return RedirectToAction();
            }
           
            var authenticatedUser = GetEntityById<UserAccount>(_authenticatedUserId);
            if (authenticatedUser == null) return Unauthorized();

            if (!authManager.IsPasswordValid(authenticatedUser, credentials.ConfirmationPassword))
            {
                TempData["AppStatus"] = AppStatus.WrongPasswordFail;
                return RedirectToAction();
            }

            if (editedCompany.Address != null && editedCompany.Address.IsIncomplete())
            {
                TempData["AppStatus"] = AppStatus.IncompleteAddressFail;
                return RedirectToAction();
            }

            var currentCompany = GetCompanyForEdit();
            if (currentCompany == null) return NotFound();

            await InitializeCompanyPayPolicy(editedCompany, editedCompanyPayPolicy);
            EditCompany(currentCompany, editedCompany);

            TempData["AppStatus"] = AppStatus.EditCompanySuccess;
            return RedirectToAction();
        }

        public async Task<IActionResult> Positions()
        {
            var currentCompany = GetCompanyForPositions();
            if (currentCompany == null) return NotFound();

            await SetDefaultViewBag();
            return View(new CompanyPositionsViewModel(currentCompany));
        }

        public async Task<IActionResult> Employees()
        {
            var currentCompany = GetCompanyForEmployees();
            if (currentCompany == null) return NotFound();

            await SetDefaultViewBag();
            return View(new CompanyEmployeesViewModel(currentCompany));
        }

        public IActionResult DownloadEmployeesXlsx()
        {
            var currentCompany = GetCompanyForEmployeesDownload();
            if (currentCompany == null) return NotFound();

            var fileContent = filesGenerator.ExportEmployeesToXlsx(currentCompany.Employees);
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var currentCulture = CultureInfo.CurrentCulture.Name;
            var fileName = currentCulture == "en-US" ? "Employees.xlsx" 
                : currentCulture == "sr-Latn-RS" ? "Zaposleni.xlsx" 
                : currentCulture == "fr-FR" ? "Employés.xlsx" : "";

            return File(fileContent, contentType, fileName);
        }

        public IActionResult DownloadEmployeesCsv()
        {
            var currentCompany = GetCompanyForEmployeesDownload();
            if (currentCompany == null) return NotFound();

            var fileContent = filesGenerator.ExportEmployeesToCsv(currentCompany.Employees);
            var contentType = "text/csv";
            var currentCulture = CultureInfo.CurrentCulture.Name;
            var fileName = currentCulture == "en-US" ? "Employees.csv" 
                : currentCulture == "sr-Latn-RS" ? "Zaposleni.csv" 
                : currentCulture == "fr-FR" ? "Employés.csv" : "";

            return File(fileContent, contentType, fileName);
        }
    }
}