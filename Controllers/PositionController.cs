using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plata.Enums;
using Plata.Data;
using Plata.Models.ViewModels;
using Plata.Models.Entities;
using Plata.Services;

namespace Plata.Controllers
{
    [Authorize]
    public partial class PositionController(LocalDbContext dbContext, AuthManager authManager, ExchangeRateApi exchangeRate) 
        : BaseController(authManager, dbContext, exchangeRate)
    {
        [HttpPost]
        public async Task<IActionResult> Create(Position newPosition)
        {
            
            newPosition.BaseGrossSalary = await exchangeRate.ConvertToBaseCurrency(newPosition.BaseGrossSalary, Request.Cookies["CurrentCurrency"]);
            AddEntity(newPosition);

            TempData["AppStatus"] = AppStatus.CreatePostionSuccess;
            return RedirectToAction("Positions", "Company");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentPosition = GetPositionForEdit(id);
            if (currentPosition == null) return NotFound();

            await SetDefaultViewBag();
            return View(new PositionEditViewModel(currentPosition));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Position editedPosition, AuthenticationViewModel credentials)
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

            var currentPosition = GetPositionForEdit(editedPosition.Id);
            if (currentPosition == null) return NotFound();

            editedPosition.BaseGrossSalary = await exchangeRate.ConvertToBaseCurrency(editedPosition.BaseGrossSalary, Request.Cookies["CurrentCurrency"]);
            EditEntity(currentPosition, editedPosition);

            TempData["AppStatus"] = AppStatus.EditPostionSuccess;
            return RedirectToAction("Edit", new { id = currentPosition.Id });
        }

        public IActionResult Delete(int id, AuthenticationViewModel credentials)
        {
            if (credentials.ConfirmationPassword == null)
            {
                TempData["AppStatus"] = AppStatus.NoConfirmationPasswordFail;
                return RedirectToAction("Edit", new { id });
            }

            var authenticatedUser = GetEntityById<UserAccount>(_authenticatedUserId);
            if (authenticatedUser == null) return Unauthorized();

            if (!authManager.IsPasswordValid(authenticatedUser, credentials.ConfirmationPassword))
            {
                TempData["AppStatus"] = AppStatus.WrongPasswordFail;
                return RedirectToAction("Edit", new { id });
            }

            var currentPosition = GetPositionForDelete(id);
            if (currentPosition == null) return NotFound();
            
            if (currentPosition.EmploymentContracts.Count > 0)
            {
                TempData["AppStatus"] = AppStatus.DeletePositionFail;
                return RedirectToAction("Edit", new { id });
            }

            DeleteEntity(currentPosition);

            TempData["AppStatus"] = AppStatus.DeletePostionSuccess;
            return RedirectToAction("Positions", "Company");
        }
    }
}
