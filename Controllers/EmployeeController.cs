using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plata.Data;
using Plata.Enums;
using Plata.Models.Entities;
using Plata.Models.ViewModels;
using Plata.Services;

namespace Plata.Controllers
{
    [Authorize]
    public partial class EmployeeController(AuthManager authManager, LocalDbContext dbContext, ExchangeRateApi exchangeRate) 
        : BaseController(authManager, dbContext, exchangeRate)
    {
        public async Task<IActionResult> Index(int id)
        {
            var currentEmployee = GetEmployeeForIndex(id);
            if (currentEmployee == null) return NotFound();

            await SetDefaultViewBag();
            return View(currentEmployee);
        }

        [HttpPost]
        public IActionResult Create(Employee newEmployee, EmploymentContract newEmployeeContract)
        {
            if (newEmployee.Address != null)
            {
                if (newEmployee.Address.IsEmpty()) newEmployee.Address = null;

                else if (newEmployee.Address.IsIncomplete())
                {
                    TempData["AppStatus"] = AppStatus.IncompleteAddressFail;
                    return RedirectToAction("Employees", "Company");
                }
            }

            newEmployee.EmploymentContracts.Add(newEmployeeContract);
            AddEntity(newEmployee);

            TempData["AppStatus"] = AppStatus.CreateEmployeeSuccess;
            return RedirectToAction("Index", new { id = newEmployee.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentEmployee = GetEmployeeForEdit(id);
            if (currentEmployee == null) return NotFound();

            await SetDefaultViewBag();
            return View(new EmployeeEditViewModel(currentEmployee));
        }

        [HttpPost]
        public IActionResult Edit(Employee editedEmployee, EmploymentContract editedEmployeeContract, AuthenticationViewModel credentials)
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

            if (editedEmployee.Address != null && editedEmployee.Address.IsIncomplete())
            {
                TempData["AppStatus"] = AppStatus.IncompleteAddressFail;
                return RedirectToAction();
            }

            var currentEmployee = GetEmployeeForEdit(editedEmployee.Id);
            if (currentEmployee == null) return NotFound();

            editedEmployee.EmploymentContracts.Add(editedEmployeeContract);
            EditEmployee(currentEmployee, editedEmployee);

            TempData["AppStatus"] = AppStatus.EditEmployeeSuccess;
            return RedirectToAction("Edit", new { id = currentEmployee.Id });
        }

        public IActionResult Delete(AuthenticationViewModel credentials, int id, string action)
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

            if (action == "Deactivate") return RedirectToAction("Deactivate", new { id });

            var currentEmployee = GetEmployeeForDelete(id);
            if (currentEmployee == null) return NotFound();

            DeleteEmployee(currentEmployee);

            TempData["AppStatus"] = AppStatus.DeleteEmployeeSuccess;
            return RedirectToAction("Employees", "Company");
        }

        public IActionResult Deactivate(int id)
        {
            var currentEmployee = GetEmployeeForDeactivate(id);
            if (currentEmployee == null) return NotFound();

            DeactivateEmployee(currentEmployee);

            TempData["AppStatus"] = AppStatus.DeactivateEmployeeSuccess;
            return RedirectToAction("Edit", new { id });
        }

        public async Task<IActionResult> Payslips(int id)
        {
            var currentEmployee = GetEmployeeForPayslips(id);
            if (currentEmployee == null) return NotFound();

            await SetDefaultViewBag();
            return View(new EmployeePayslipsViewModel(currentEmployee)); 
        }
    }
}